"""
Fitter for pairwise state tomography
"""

from ast import literal_eval
from itertools import combinations, product

from qiskit.ignis.verification.tomography import StateTomographyFitter
from qiskit.ignis.verification.tomography.data import marginal_counts
from qiskit.ignis.verification.tomography.basis.circuits import _format_registers

from qiskit.quantum_info.analysis.average import average_data

_OBSERVABLE_FIRST = {'00': 1, '01': -1, '10': 1, '11': -1}
_OBSERVABLE_SECOND = {'00': 1, '01': 1, '10': -1, '11': -1}
_OBSERVABLE_CORRELATED = {'00': 1, '01': -1, '10': -1, '11': 1}

class PairwiseStateTomographyFitter(StateTomographyFitter):
    """
    Pairwise Maximum-likelihood estimation state tomography fitter
    """

    def __init__(self, result, circuits, measured_qubits):
        """
        Initialize state tomography fitter with experimental data.

        Args:
            result (Result): a Qiskit Result object obtained from executing
                            pairwise tomography circuits.
            circuits (list): a list of circuits or circuit names to extract
                            count information from the result object.
            measured_qubits (list): a list of indices of the measured qubits
                            (corresponding to the tomography circuits)
        """
        self._circuits = circuits
        self._result = result

        if isinstance(measured_qubits, list):
            #Unroll list of registers
            meas_qubits = _format_registers(*measured_qubits)
        else:
            meas_qubits = _format_registers(measured_qubits)

        self._qubit_list = meas_qubits

        self._meas_basis = None
        self._prep_basis = None
        super().set_measure_basis("Pauli")
        super().set_preparation_basis("Pauli")
        self._data = {}

    def fit(self, method='auto', standard_weights=True, beta=0.5, pairs_list=None, **kwargs):
        """
        Reconstruct pairwise quantum states using CVXPY convex optimization.

        Args:
            pairs_list (list): A list of tuples containing the indices of the
                               qubit pairs for which to perform tomography
            output (str): 'density_matrix' (default), for obtaining the density
                          matrix, 'expectation' for the Pauli expectation values
            **kwargs (optional): kwargs for fitter method,
            see BaseTomographyFitter

        Returns:
            A dictionary of the form {(i, j): obj}, where 
            obj = rho(i,j) is two-qubit density matrix for qubits i, j if 
            output = 'density_matrix' (default). If output = 'expectation',
            obj = {('X', 'X'): <XX>, ('X', 'Y'): <XY>, ...}, where <.> is the 
            expectation value of the two-qubit operator.
        """
        # If no list of pairs provided, then evaluate for all qubit pairs
        if not pairs_list:
            indices = range(len(self._qubit_list))
            pairs_list = list(combinations(indices, 2))

        result = {}

        for p in pairs_list:
            result[p] = self._fit_ij(*p,
                                    method=method,
                                    standard_weights=standard_weights,
                                    beta=beta,
                                    **kwargs)

        return result

    def _fit_ij(self, i, j, output='density_matrix', **kwargs):
        """
            Returns the tomographic reconstruction for the qubits i and j

            Args:
            i (int): first qubit
            j (int): second qubit
            output (str): 'density_matrix' (default) for returning density 
                          matrix, 'expectation' for Pauli operators expectation
                          values.
        """
        assert i != j, "i and j must be different"

        # Get the layer of interest in the list of circuits
        l = self._find_layer(i, j)

        # Take the circuits of interest
        circuits = self._circuits[0:3]
        circuits += self._circuits[(3 + 6*l) : (3 + 6*(l+1))]

        # This will create an empty _data dict for the fit function
        # We are using a member field so that  we can use the super() fit 
        # function
        self._data = {}

        # Process measurement counts into probabilities
        for circ in circuits:
            # Take only the relevant qubit labels from the circuit label
            tup = literal_eval(circ.name)
            tup = (tup[i], tup[j])

            # Marginalize the counts for the two relevant qubits
            counts = marginal_counts(self._result.get_counts(circ), [i, j])

            # Populate the data
            self._data[tup] = counts

        # Check that all the required measurements are there
        expected_corr = product(['X', 'Y', 'Z'], ['X', 'Y', 'Z'])
        if set(self._data.keys()) != set(expected_corr):
            raise Exception("Could not find all the measurements required for tomography")

        if output == 'density_matrix':
            # Do the actual fit using StateTomographyFitter base method
            result = super().fit(**kwargs)
        elif output == 'expectation':
            # Return the expectation values
            result = self._evaluate_expectation()
        else:
            raise ValueError("Output must be either 'density_matrix' or 'expectation'")

        # clear the _data field
        self._data = None
        return result

    def _evaluate_expectation(self):
        """
        Utility function for evaluating expectation value of two-qubit Pauli
        measurements.

        Returns:
            A dict where keys are pairs of Pauli operators, e.g. ('X', 'Z') 
            or ('I', 'X'), and values are the expectation values.
        """

        paulis = ['I', 'X', 'Y', 'Z']
        keys = product(paulis, paulis)

        result = {}
        for key in keys:
            if key[0] == 'I':
                if key[1] == 'I':
                    pass
                else:
                    result[key] = average_data(self._data[('Z', key[1])], _OBSERVABLE_SECOND)
            elif key[1] == 'I':
                result[key] = average_data(self._data[(key[0], 'Z')], _OBSERVABLE_FIRST)
            else:
                result[key] = average_data(self._data[key], _OBSERVABLE_CORRELATED)

        return result

    def _find_layer(self, i, j):
        """
        Utility function for finding the position of the circuits in the circuit
        list returned by pairwise_state_tomography()
        """
        l = 0
        while int(i/3**l) % 3 == int(j/3**l) % 3:
            l += 1
        return l
