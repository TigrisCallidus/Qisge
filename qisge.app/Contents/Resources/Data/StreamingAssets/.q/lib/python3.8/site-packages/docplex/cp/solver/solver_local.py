# --------------------------------------------------------------------------
# Source file provided under Apache License, Version 2.0, January 2004,
# http://www.apache.org/licenses/
# (c) Copyright IBM Corp. 2016, 2017, 2018
# --------------------------------------------------------------------------
# Author: Olivier OUDOT, IBM Analytics, France Lab, Sophia-Antipolis

"""
This module allows to solve a model expressed as a CPO file using
a local CP Optimizer Interactive (cpoptimizer(.exe)).
"""

from docplex.cp.solution import *
from docplex.cp.utils import CpoException
import docplex.cp.solver.solver as solver

import subprocess
import sys
import time
import threading
import json
import signal, os


###############################################################################
##  Private constants
###############################################################################

# List of command ids that can be sent to solver
CMD_EXIT            = "Exit"           # End process (no data)
CMD_SET_CPO_MODEL   = "SetCpoModel"    # CPO model as string
CMD_SOLVE_MODEL     = "SolveModel"     # Complete solve of the model (no data)
CMD_START_SEARCH    = "StartSearch"    # Start search (no data)
CMD_SEARCH_NEXT     = "SearchNext"     # Get next solution (no data)
CMD_END_SEARCH      = "EndSearch"      # End search (no data)
CMD_REFINE_CONFLICT = "RefineConflict" # Refine conflict (no data)
CMD_PROPAGATE       = "Propagate"      # Propagate (no data)
CMD_RUN_SEEDS       = "RunSeeds"       # Run with multiple seeds.
CMD_ADD_CALLBACK    = "AddCallback"    # Add callback proxy to the solver

# List of events received from solver
EVT_VERSION_INFO        = "VersionInfo"        # Local solver version info (String in JSON format)
EVT_SUCCESS             = "Success"            # Success in last command execution
EVT_ERROR               = "Error"              # Error (data is error string)
EVT_TRACE               = "DebugTrace"         # Debugging trace
EVT_SOLVER_OUT_STREAM   = "OutStream"          # Solver output stream
EVT_SOLVER_WARN_STREAM  = "WarningStream"      # Solver warning stream
EVT_SOLVER_ERR_STREAM   = "ErrorStream"        # Solver error stream
EVT_SOLVE_RESULT        = "SolveResult"        # Solver result in JSON format
EVT_CONFLICT_RESULT     = "ConflictResult"     # Conflict refiner result in JSON format
EVT_CONFLICT_RESULT_CPO = "ConflictResultCpo"  # Conflict refiner result in CPO format
EVT_PROPAGATE_RESULT    = "PropagateResult"    # Propagate result in JSON format
EVT_RUN_SEEDS_RESULT    = "RunSeedsResult"     # Run seeds result (no data, all is in log)
EVT_CALLBACK_EVENT      = "CallbackEvent"      # Callback event. Data is event name.
EVT_CALLBACK_DATA       = "CallbackData"       # Callback data, following event. Data is JSON document.

# Max possible received data size in one message
_MAX_RECEIVED_DATA_SIZE = 1000000

# Python 2 indicator
IS_PYTHON_2 = (sys.version_info[0] == 2)

# Version of this client
CLIENT_VERSION = 3


###############################################################################
##  Public classes
###############################################################################

class LocalSolverException(CpoException):
    """ The base class for exceptions raised by the local solver client
    """
    def __init__(self, msg):
        """ Create a new exception
        Args:
            msg: Error message
        """
        super(LocalSolverException, self).__init__(msg)


class CpoSolverLocal(solver.CpoSolverAgent):
    """ Interface to a local solver through an external process """

    def __init__(self, solver, params, context):
        """ Create a new solver that solves locally with CP Optimizer Interactive.

        Args:
            solver:  Parent solver
            params:  Solving parameters
            context: Solver context
        Raises:
            CpoException if proxy executable does not exists
        """
        # Call super
        self.process = None
        self.active = True
        super(CpoSolverLocal, self).__init__(solver, params, context)

        # Check if executable file exists
        if context.execfile is None:
            raise CpoException("Executable file should be given in 'execfile' context attribute.")
        if not is_string(context.execfile):
            raise CpoException("Executable file should be given in 'execfile' as a string.")
        #if not os.path.isfile(context.execfile):
        #    raise CpoException("Executable file '" + str(context.execfile) + "' does not exists")

        # Create solving process
        cmd = [context.execfile]
        if context.parameters is not None:
            cmd.extend(context.parameters)
        context.log(2, "Solver exec command: '", ' '.join(cmd), "'")
        try:
            self.process = subprocess.Popen(cmd, stdin=subprocess.PIPE, stderr=subprocess.STDOUT, stdout=subprocess.PIPE, universal_newlines=False)
        except:
            raise CpoException("Can not execute command '{}'. Please check availability of required executable file.".format(' '.join(cmd)))
        self.pout = self.process.stdin
        self.pin = self.process.stdout

        # Read initial version info from process
        self.version_info = None
        timer = threading.Timer(1, lambda: self.process.kill() if self.version_info is None else None)
        timer.start()
        evt, data = self._read_message()
        timer.cancel()
        if evt != EVT_VERSION_INFO:
            raise LocalSolverException("Unexpected event {} received instead of version info event {}.".format(evt, EVT_VERSION_INFO))
        self.version_info = verinf = json.loads(data)
        self.available_command = self.version_info['AvailableCommands']
        # Normalize information
        verinf['AgentModule'] = __name__

        context.log(3, "Local solver info: '", verinf, "'")

        # Check solver version if any
        sver = self.version_info.get('SolverVersion')
        mver = solver.get_model_format_version()
        if sver and mver and compare_natural(mver, sver) > 0:
            raise LocalSolverException("Solver version {} is lower than model format version {}.".format(sver, mver))

        # Send CPO model to process
        cpostr = self._get_cpo_model_string()
        self._write_message(CMD_SET_CPO_MODEL, cpostr)
        self._wait_json_result(EVT_SUCCESS)  # JSON stored
        context.log(3, "Model sent.")

        # Initialize CPO callback setting
        self.callback_added = False


    def __del__(self):
        # End solve
        self.end()


    def solve(self):
        """ Solve the model

        According to the value of the context parameter 'verbose', the following information is logged
        if the log output is set:
         * 1: Total time spent to solve the model
         * 2: The process exec file
         * 3: Content of the JSON response
         * 4: Solver traces (if any)
         * 5: Messages sent/receive to/from process

        Returns:
            Model solve result,
            object of class :class:`~docplex.cp.solution.CpoSolveResult`.
        """
        # Add callback if needed
        self._add_callback_if_needed()

        # Start solve
        self._write_message(CMD_SOLVE_MODEL)

        # Wait JSON result
        jsol = self._wait_json_result(EVT_SOLVE_RESULT)

        # Build result object
        return self._create_result_object(CpoSolveResult, jsol)


    def start_search(self):
        """ Start a new search. Solutions are retrieved using method search_next().
        """
        # Add callback if needed
        self._add_callback_if_needed()

        self._write_message(CMD_START_SEARCH)


    def search_next(self):
        """ Get the next available solution.

        (This method starts search automatically.)

        Returns:
            Next model result (type CpoSolveResult)
        """

        # Request next solution
        self._write_message(CMD_SEARCH_NEXT)

        # Wait JSON result
        jsol = self._wait_json_result(EVT_SOLVE_RESULT)

        # Build result object
        return self._create_result_object(CpoSolveResult, jsol)


    def end_search(self):
        """ End current search.

        Returns:
            Last (fail) solve result with last solve information (type CpoSolveResult)
        """

        # Request end search
        self._write_message(CMD_END_SEARCH)

        # Wait JSON result
        jsol = self._wait_json_result(EVT_SOLVE_RESULT)

        # Build result object
        return self._create_result_object(CpoSolveResult, jsol)


    def abort_search(self):
        """ Abort current search.
        This method is designed to be called by a different thread than the one currently solving.
        """
        self.end()


    def refine_conflict(self):
        """ This method identifies a minimal conflict for the infeasibility of the current model.

        See documentation of :meth:`~docplex.cp.solver.solver.CpoSolver.refine_conflict` for details.

        Returns:
            Conflict result,
            object of class :class:`~docplex.cp.solution.CpoRefineConflictResult`.
        """
        # Ensure cpo model is generated with all constraints named
        if not self.context.model.name_all_constraints:
            self.context.model.name_all_constraints = True
            cpostr = self._get_cpo_model_string()
            self.context.model.name_all_constraints = False
            # Send CPO model to process
            self._write_message(CMD_SET_CPO_MODEL, cpostr)
            self._wait_event(EVT_SUCCESS)

        # Add callback if needed
        self._add_callback_if_needed()
        
        # Check if cpo format required
        pver = self.version_info.get('ProxyVersion')
        if self.context.add_conflict_as_cpo and pver and (int(pver) >= 9):
            # Request refine conflict with CPO format
            self._write_message(CMD_REFINE_CONFLICT, bytearray([1]))
            # Wait JSON result
            jsol = self._wait_json_result(EVT_CONFLICT_RESULT)
            # Wait for CPO conflict
            cposol = self._wait_event(EVT_CONFLICT_RESULT_CPO)
        else:
            # Request refine conflict
            self._write_message(CMD_REFINE_CONFLICT)
            # Wait JSON result
            jsol = self._wait_json_result(EVT_CONFLICT_RESULT)
            # No CPO conflict
            cposol = None

        # Build result object
        result = self._create_result_object(CpoRefineConflictResult, jsol)
        result.cpo_conflict = cposol
        return result


    def propagate(self):
        """ This method invokes the propagation on the current model.

        See documentation of :meth:`~docplex.cp.solver.solver.CpoSolver.propagate` for details.

        Returns:
            Propagation result,
            object of class :class:`~docplex.cp.solution.CpoSolveResult`.
        """
        # Add callback if needed
        self._add_callback_if_needed()

        # Request propagation
        self._write_message(CMD_PROPAGATE)

        # Wait JSON result
        jsol = self._wait_json_result(EVT_PROPAGATE_RESULT)

        # Build result object
        return self._create_result_object(CpoSolveResult, jsol)


    def run_seeds(self, nbrun):
        """ This method runs *nbrun* times the CP optimizer search with different random seeds
        and computes statistics from the result of these runs.

        This method does not return anything. Result statistics are displayed on the log output
        that should be activated.

        Each run of the solver is stopped according to single solve conditions (TimeLimit for example).
        Total run time is then expected to take *nbruns* times the duration of a single run.

        Args:
            nbrun: Number of runs with different seeds.
        Returns:
            Run result, object of class :class:`~docplex.cp.solution.CpoRunResult`.
        Raises:
            CpoNotSupportedException: method not available in local solver.
        """
        # Add callback if needed
        self._add_callback_if_needed()

        # Check command availability
        if CMD_RUN_SEEDS not in self.available_command:
            raise CpoNotSupportedException("Method 'run_seeds' is not available in local solver '{}'".format(self.context.execfile))

        # Request run seeds
        nbfrm = bytearray(4)
        nbfrm[0] = (nbrun >> 24) & 0xFF
        nbfrm[1] = (nbrun >> 16) & 0xFF
        nbfrm[2] = (nbrun >> 8)  & 0xFF
        nbfrm[3] = nbrun         & 0xFF

        self._write_message(CMD_RUN_SEEDS, data=nbfrm)

        # Wait result
        self._wait_event(EVT_RUN_SEEDS_RESULT)

        # Build result object
        return self._create_result_object(CpoRunResult)


    def end(self):
        """ End solver and release all resources.
        """
        if self.active:
            self.active = False
            try:
                self._write_message(CMD_EXIT)
            except:
                pass
            try:
                self.pout.close()
            except:
                pass
            try:
                self.pin.close()
            except:
                pass
            try:
                self.process.kill()
            except:
                pass
            try:
                self.process.wait()
            except:
                pass
            self.process = None
            super(CpoSolverLocal, self).end()


    def _wait_event(self, xevt):
        """ Wait for a particular event while forwarding logs if any.
        Args:
            xevt: Expected event
        Returns:
            Message data
        Raises:
            LocalSolverException if an error occurs
        """
        # Initialize first error string to enrich exception if any
        firsterror = None

        # Read events
        while True:
            # Read and process next message
            evt, data = self._read_message()

            if evt == xevt:
                return data

            elif evt in (EVT_SOLVER_OUT_STREAM, EVT_SOLVER_WARN_STREAM):
                if data:
                    # Store log if required
                    if self.log_enabled:
                        self._add_log_data(data)

            elif evt == EVT_SOLVER_ERR_STREAM:
                if data:
                    if firsterror is None:
                        firsterror = data.replace('\n', '')
                    out = self.log_output if self.log_output is not None else sys.stdout
                    out.write("ERROR: {}\n".format(data))
                    out.flush()

            elif evt == EVT_TRACE:
                self.context.log(4, "ANGEL: " + data)

            elif evt == EVT_ERROR:
                if firsterror is not None:
                    data += " (" + firsterror + ")"
                self.end()
                raise LocalSolverException("Solver error: " + data)

            elif evt == EVT_CALLBACK_EVENT:
                event = data
                # Read data
                evt, data = self._read_message()
                assert evt == EVT_CALLBACK_DATA
                res = self._create_result_object(CpoSolveResult, data)
                self.solver._notify_callback_event(event, res)

            else:
                self.end()
                raise LocalSolverException("Unknown event received from local solver: " + str(evt))


    def _wait_json_result(self, evt):
        """ Wait for a JSON result while forwarding logs if any.
        Args:
            evt: Event to wait for
        Returns:
            JSON solution string, decoded from UTF8
        """

        # Wait JSON result
        data = self._wait_event(evt)

        # Store last json result
        self._set_last_json_result_string(data)
        self.context.log(3, "JSON result:\n", data)

        return self.last_json_result


    def _write_message(self, cid, data=None):
        """ Write a message to the solver process
        Args:
            cid:   Command name
            data:  Data to write, already encoded in UTF8 if required
        """
        # Encode elements
        stime = time.time()
        cid = cid.encode('utf-8')
        if is_string(data):
            data = data.encode('utf-8')
        nstime = time.time()
        self.process_infos.incr(CpoProcessInfos.TOTAL_UTF8_ENCODE_TIME, nstime - stime)

        # Build header
        tlen = len(cid)
        if data is not None:
            tlen += len(data) + 1
        if tlen > 0xffffffff:
            raise LocalSolverException("Try to send a message with length {}, greater than {}.".format(tlen, 0xffffffff))
        frame = bytearray(6)
        frame[0] = 0xCA
        frame[1] = 0xFE
        frame[2] = (tlen >> 24) & 0xFF
        frame[3] = (tlen >> 16) & 0xFF
        frame[4] = (tlen >> 8)  & 0xFF
        frame[5] = tlen         & 0xFF

        # Add data if any
        self.context.log(5, "Send message: cmd=", cid, ", tsize=", tlen)
        if data is None:
            frame = frame + cid
        else:
            frame = frame + cid + bytearray(1) + data

        # Write message frame
        self.pout.write(frame)
        self.pout.flush()

        # Update statistics
        self.process_infos.incr(CpoProcessInfos.TOTAL_DATA_SEND_TIME, time.time() - nstime)
        self.process_infos.incr(CpoProcessInfos.TOTAL_DATA_SEND_SIZE, len(frame))


    def _read_message(self):
        """ Read a message from the solver process
        Returns:
            Tuple (evt, data)
        """
        # Read message header
        frame = self._read_frame(6)
        if (frame[0] != 0xCA) or (frame[1] != 0xFE):
            erline = frame + self._read_error_message()
            erline = erline.decode()
            self.end()
            raise LocalSolverException("Invalid message header. Possible error generated by solver: " + erline)

        # Read message data
        tsize = (frame[2] << 24) | (frame[3] << 16) | (frame[4] << 8) | frame[5]
        data = self._read_frame(tsize)

        # Split name and data
        ename = 0
        while (ename < tsize) and (data[ename] != 0):
            ename += 1

        # Decode name and data
        stime = time.time()
        if ename == tsize:
            # Command only, no data
            evt = data.decode('utf-8')
            data = None
        else:
            # Split command and data
            evt = data[0:ename].decode('utf-8')
            data = data[ename+1:].decode('utf-8')

        # Update statistics
        self.process_infos.incr(CpoProcessInfos.TOTAL_UTF8_DECODE_TIME, time.time() - stime)
        self.process_infos.incr(CpoProcessInfos.TOTAL_DATA_RECEIVE_SIZE, tsize + 6)

        # Log received message
        self.context.log(5, "Read message: ", evt, ", data: '", data, "'")

        return evt, data


    def _read_frame(self, nbb):
        """ Read a byte frame from input stream
        Args:
            nbb:  Number of bytes to read
        Returns:
            Byte array
        """
        # Read data
        data = self.pin.read(nbb)
        if len(data) != nbb:
            if len(data) == 0:
                # Check if first read of data
                if self.process_infos.get(CpoProcessInfos.TOTAL_DATA_RECEIVE_SIZE, 0) == 0:
                    if IS_WINDOWS:
                        raise LocalSolverException("Nothing to read from local solver process. Possibly not started because cplex dll is not accessible.")
                    else:
                        raise LocalSolverException("Nothing to read from local solver process. Check its availability.")
                else:
                    try:
                        self.process.wait()
                        rc = self.process.returncode
                    except:
                        rc = "unknown"
                    raise LocalSolverException("Nothing to read from local solver process. Process seems to have been stopped (rc={}).".format(rc))
            else:
                raise LocalSolverException("Read only {} bytes when {} was expected.".format(len(data), nbb))

        # Return
        if IS_PYTHON_2:
            data = bytearray(data)
        return data


    def _read_error_message(self):
        """ Read stream to search for error line end. Called when wrong input is detected,
        to try to read an "Assertion failed" message for example.
        Returns:
            Byte array
        """
        data = []
        bv = self.pin.read(1)
        if IS_PYTHON_2:
            while (bv != '') and (bv != '\n'):
                data.append(ord(bv))
                bv = self.pin.read(1)
                data = bytearray(data)
        else:
            while (bv != b'') and (bv != b'\n'):
                data.append(ord(bv))
                bv = self.pin.read(1)

        return bytearray(data)


    def _add_callback_if_needed(self):
        """ Ask solver to add callback if needed. """
        if not self.callback_added and self.solver.callbacks:
            # Check solver version
            aver = self.version_info.get('AngelVersion', 0)
            sver = self.version_info.get('SolverVersion', "1")
            if (compare_natural(sver, "12.10") >= 0) and (aver >= 8):
                self._write_message(CMD_ADD_CALLBACK)
                self._wait_event(EVT_SUCCESS)
                self.callback_added = True
                self.context.log(3, "CPO callback created.")
            else:
                raise LocalSolverException("This version of the CPO solver does not support solver callbacks. Solver: {}, agent: {}".format(sver, aver))


###############################################################################
##  Public functions
###############################################################################

from docplex.cp.model import CpoModel

def get_solver_info():
    """ Get the information data of the local CP solver that is target by the solver configuration.

    This method creates a CP solver to retrieve this information, and end it immediately.
    It returns a dictionary with various information, as in the following example:
    ::
    {
       "AngelVersion" : 5,
       "SourceDate" : "Sep 12 2017",
       "SolverVersion" : "12.8.0.0",
       "IntMin" : -9007199254740991,
       "IntMax" : 9007199254740991,
       "IntervalMin" : -4503599627370494,
       "IntervalMax" : 4503599627370494,
       "AvailableCommands" : ["Exit", "SetCpoModel", "SolveModel", "StartSearch", "SearchNext", "EndSearch", "RefineConflict", "Propagate", "RunSeeds"]
    }

    Returns:
        Solver information dictionary, or None if not available.
    """
    try:
        with solver.CpoSolver(CpoModel()) as slvr:
            if isinstance(slvr.agent, CpoSolverLocal):
                return slvr.agent.version_info
    except:
        pass
    return None




