# --------------------------------------------------------------------------
# Source file provided under Apache License, Version 2.0, January 2004,
# http://www.apache.org/licenses/
# (c) Copyright IBM Corp. 2015, 2016
# --------------------------------------------------------------------------

# gendoc: ignore

import re
import six


class ExchangeFormat(object):
    """ The ExchangeFormat class groups data and attributes of model formats.

        This class is not meant to be instantiated by users.
    """

    def __init__(self, name, extension, requires_cplex, is_binary=False):
        assert isinstance(name, str)
        assert isinstance(extension, str)
        self._name = name
        self._extension = extension if extension.startswith(".") else ".%s" % extension
        self._requires_cplex = bool(requires_cplex)
        self._is_binary = is_binary

    @property
    def filetype(self):
        # remove starting '.' from extension
        return self._extension[1:]

    @property
    def name(self):
        """ Returns a string qualifying the format to be used in messages.

        Example:
            LP format name is "LP"
        """
        return self._name

    @property
    def is_binary(self):
        """ Returns True if the format is binary, False for a text format.

        Example:
            LP is a text format and returns False.

            SAV is a binary format and returns TRue
        """
        return self._is_binary

    @property
    def extension(self):
        """ Returns the full file extension of the format, including "."

        Example:
            LP format extension is ".lp". SAV format extension is ".sav"
        """
        return self._extension

    def to_string(self):
        return "%s_format" % self.name

    def __str__(self):
        return self.to_string()

    def __repr__(self):
        return self.to_string()

    @staticmethod
    def fromstring(definition):
        if definition:
            definition = definition.lower()
        return _FORMAT_MAPPER.get(definition, None)


class LPFormat(ExchangeFormat):

    __raw = " -+/\\<>"
    __cooked = "_mp____"

    from docplex.mp.compat23 import mktrans

    _str_translate_table = mktrans(__raw, __cooked)
    _unicode_translate_table = {}
    for c in range(len(__raw)):
        _unicode_translate_table[ord(__raw[c])] = ord(__cooked[c])

    @classmethod
    def _translate_chars_py2(cls, raw_name):
        # noinspection PyUnresolvedReferences
        if isinstance(raw_name, unicode):
            char_mapping = cls._unicode_translate_table
        else:
            char_mapping = cls._str_translate_table
        return raw_name.translate(char_mapping)

    @classmethod
    def _translate_chars_py3(cls, raw_name):
        return raw_name.translate(cls._unicode_translate_table)

    # which translate_method to use
    if six.PY2:
        _translate_chars = _translate_chars_py2
    else:  # pragma: no cover
        _translate_chars = _translate_chars_py3

    def __init__(self):
        ExchangeFormat.__init__(self, "LP", "lp", requires_cplex=False)

    lp_re = re.compile(r"[a-df-zA-DF-Z!#$%&()/,;?@_`'{}|\"][a-zA-Z0-9!#$%&()/.,;?@_`'{}|\"]*")

    @classmethod
    def is_lp_compliant(cls, name, lp_re=lp_re):
        if name:
            lp_match = lp_re.match(name)
            return lp_match and lp_match.start() == 0 and lp_match.end() == len(name)
        else:
            return False

    @classmethod
    def make_prefix_name(cls, prefix, local_index, offset=1):
        prefixed_name = "{0:s}{1:d}".format(prefix, local_index + offset)
        return prefixed_name

    @classmethod
    def lp_var_name(cls, dvar):
        return cls.lp_name(dvar.get_name(), "x", dvar.get_index())

    # @classmethod
    # def lp_ct_name(cls, linct):
    #     return cls.lp_name(linct.get_name(), "c", linct.get_index())

    @classmethod
    def lp_name(cls, raw_name, prefix, local_index, hide_names=False,
                noncompliant_hook=None):
        # anonymous constraints must be named in a LP (we follow CPLEX here)
        if hide_names or not raw_name:
            return cls.make_prefix_name(prefix, local_index, offset=1)

        # # swap blanks with underscores
        ws_name = cls._translate_chars(raw_name)

        if not cls.is_lp_compliant(ws_name):
            if ws_name[0] in 'eE':
                # fixing eE non-LP names
                fixed_name = '_' + ws_name
                if cls.is_lp_compliant(fixed_name):
                    return fixed_name
            # -- stats
            if noncompliant_hook:
                noncompliant_hook(raw_name)
            # --
            return cls.make_prefix_name(prefix, local_index, offset=1)
        else:
            # truncate if necessary, again this does nothing if name is too short
            return ws_name[:255]


""" The global LP format object."""
# noinspection PyPep8
LP_format  = LPFormat()
SAV_format = ExchangeFormat("SAV", "sav", requires_cplex=True, is_binary=True)
OPL_format = ExchangeFormat("OPL", ".mod", requires_cplex=False)
MPS_format = ExchangeFormat("MPS", ".mps", requires_cplex=True)

_FORMAT_MAPPER = {"lp": LP_format,
                  "opl": OPL_format
                  }
