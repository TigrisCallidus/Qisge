# --------------------------------------------------------------------------
# Source file provided under Apache License, Version 2.0, January 2004,
# http://www.apache.org/licenses/
# (c) Copyright IBM Corp. 2015, 2016
# --------------------------------------------------------------------------

# gendoc: ignore
from docplex.mp.utils import resolve_caller_as_string, is_number, is_ordered_sequence
from docplex.mp.constants import BasisStatus

import math


class StaticTypeChecker(object):

    @staticmethod
    def typecheck_as_power(mdl, e, power):
        # INTERNAL: checks <power> is 0,1,2
        if power < 0 or power > 2:
            mdl.fatal("Cannot raise {0!s} to the power {1}. A variable's exponent must be 0, 1 or 2.", e, power)

    @staticmethod
    def cannot_be_used_as_denominator_error(mdl, denominator, numerator):
        mdl.fatal("{1!s} / {0!s} : operation not supported, only numbers can be denominators", denominator, numerator)

    @classmethod
    def typecheck_as_denominator(cls, mdl, denominator, numerator):
        if not is_number(denominator):
            cls.cannot_be_used_as_denominator_error(mdl, denominator, numerator)
        else:
            float_e = float(denominator)
            if 0 == float_e:
                mdl.fatal("Zero divide on {0!s}", numerator)

    @classmethod
    def typecheck_discrete_expression(cls, logger, expr, msg):
        if not expr.is_discrete():
            logger.fatal('{0}, expression: ({1!s}) is not discrete', msg, expr)

    @classmethod
    def typecheck_discrete_constraint(cls, logger, ct, msg):
        if not ct.is_discrete():
            logger.fatal('{0}, {1!s} is not discrete', msg, ct)

    @classmethod
    def mul_quad_lin_error(cls, logger, f1, f2):
        logger.fatal(
            "Cannot multiply {0!s} by {1!s}, some terms would have degree >= 3. Maximum polynomial degree is 2.",
            f1, f2)

    @classmethod
    def typecheck_callable(cls, logger, arg, msg):
        if not callable(arg):
            logger.fatal(msg)

    @classmethod
    def typecheck_num_nan_inf(cls, logger, arg, caller=None):
        # check for a "real" number, not a NaN, not infinity
        caller_string = "{0}: ".format(caller) if caller is not None else ""
        if not is_number(arg):
            logger.fatal("{0}Expecting number, {1!r} was passed", caller_string, arg)
        elif math.isnan(arg):
            logger.fatal("{0}NaN value was passed", caller_string)
        elif math.isinf(arg):
            logger.fatal("{0}Infinite value was passed", caller_string)

    @classmethod
    def check_number_pair(cls, logger, arg, caller=None):
        caller_string = "{0}: ".format(caller) if caller is not None else ""
        if arg is None:
            logger.fatal("{0}expecting 2-tuple of floats, {1!r} was passed", caller_string, arg)
        if isinstance(arg, tuple):
            if len(arg) != 2:
                logger.fatal("{0}expecting 2-tuple of floats, invalid tuple {1!r} was passed",
                             caller_string, arg)
            cls.typecheck_num_nan_inf(logger, arg[0])
            cls.typecheck_num_nan_inf(logger, arg[1])
        else:
            logger.fatal("{0}expecting 2-tuple of floats, {1!r} was passed", caller_string, arg)

    @classmethod
    def check_file(cls, logger, path, name, expected_extensions, caller=None):
        import os
        if not os.path.exists(path):
            raise IOError("{0} file not found: {1}".format(name, path))
        # find extension
        filename, file_extension = os.path.splitext(path)
        if file_extension not in expected_extensions:
            caller_string = '' if not caller else caller + ' '
            logger.warning("{0}Unexpected file extension: {1}, expecting one of {2}", caller_string,
                           file_extension, "|".join(x for x in expected_extensions))

    @classmethod
    def typecheck_initial_lp_stats(cls, logger, stats, stat_type, caller=None):
        caller_s = resolve_caller_as_string(caller)
        if not is_ordered_sequence(stats):
            logger.fatal('{0}expects ordered sequence of {2} basis statuses, {1!r} was passed', caller_s, stats, stat_type)
        l_stats = list(stats)
        for s, stat in enumerate(l_stats):
            if not isinstance(stat, BasisStatus):
                logger.fatal('{0}expects a sequence of {3} basis status, {1} was passed at pos {2}',
                             caller_s, stat, s, stat_type)
        return l_stats

    @classmethod
    def typecheck_logical_op(cls, logger, arg, caller):
        if not hasattr(arg, 'as_logical_operand') or arg.as_logical_operand() is None:
            caller_s = resolve_caller_as_string(caller)
            logger.fatal('{1}Not a logical operand: {0!s} - Expecting binary variable, logical expression or constraint',
                         arg, caller_s)


