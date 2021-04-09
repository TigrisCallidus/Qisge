# --------------------------------------------------------------------------
# Source file provided under Apache License, Version 2.0, January 2004,
# http://www.apache.org/licenses/
# (c) Copyright IBM Corp. 2015, 2016
# --------------------------------------------------------------------------

"""
IBM Decision Optimization CPLEX Modeling for Python - Constraint Programming

This package contains a Python API allowing to build Constraint Programming
models and their solving using the Decision Optimization cloud services.
"""

import platform
import sys
import docplex.version as dcpv
import warnings

# Check platform system
psyst = platform.system()
if psyst.lower() not in ('darwin', 'linux', 'windows', 'microsoft', 'aix'):
    msg = "DOcplex.CP is supported on Linux, Windows, Darwin and AIX, not on '{}'. Use it at your own risk.".format(psyst)
    warnings.warn(msg, RuntimeWarning)

# Check version of Python
pv = sys.version_info
if (pv < (2, 7)) or ((pv[0] == 3) and (pv < (3, 4) or pv >= (3, 8))):
    msg = "DOcplex.CP is supported by Python versions 2.7.9+, 3.4.x, to 3.7.x, not '{}'. Use it at your own risk."\
        .format('.'.join(str(x) for x in pv))
    warnings.warn(msg, RuntimeWarning)

# Set version information
__version_info__ = (dcpv.docplex_version_major, dcpv.docplex_version_minor, dcpv.docplex_version_micro)
