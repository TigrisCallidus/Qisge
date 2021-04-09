"""
Utility functions
"""

import numpy as np
import scipy.linalg as la
from scipy.optimize import minimize
#from scipy.special import xlogy
from qiskit.quantum_info import partial_trace, entropy, mutual_information
from qiskit.tools import outer

def concurrence(state):
    """Calculate the concurrence.

    Args:
        state (np.array): a quantum state (1x4 array) or a density matrix (4x4
                          array)
    Returns:
        float: concurrence.
    Raises:
        Exception: if attempted on more than two qubits.
    """
    rho = np.array(state)
    if rho.ndim == 1:
        rho = outer(state)
    if len(state) != 4:
        raise Exception("Concurrence is only defined for more than two qubits")

    YY = np.fliplr(np.diag([-1, 1, 1, -1]))
    A = rho.dot(YY).dot(rho.conj()).dot(YY)
    w = la.eigvals(A)
    w = np.sort(np.real(w))
    w = np.sqrt(np.maximum(w, 0))
    return max(0.0, w[-1] - np.sum(w[0:-1]))


_s0 = np.array([[1, 0], [0, 1]], dtype=complex)
_sx = np.array([[0, 1], [ 1, 0]], dtype=complex)
_sy = np.array([[0, -1j], [1j, 0]], dtype=complex)
_sz = np.array([[1, 0], [0, -1]], dtype=complex)
_paulis = np.array([_s0, _sx, _sy, _sz])

def n_vector(theta, phi):
    """
    Unit vector for the specified direction
    """

    n = np.array([np.sin(theta)*np.cos(phi), np.sin(theta)*np.sin(phi), np.cos(theta)])
    return n

def projective_measurement(theta, phi, qubit=0):
    """
    Returns a projective measurement along the specified direction for one of
    the two qubits.

    Args:
        theta (float): "latitude" angle for the direction of the measurement
        phi (float): "longitude" angle for the direction of the measurement
        qubit (int): 0 or 1, the qubit on which the measurement is done (default 0)

    Returns:
        list: The two measurement operators (numpy Arrays)
    """
    n = n_vector(theta, phi)
    Pi1 = 0.5 * _s0
    Pi2 = 0.5 * _s0
    for i in range(3):
        Pi1 += 0.5 * n[i] * _paulis[i+1]
        Pi2 -= 0.5 * n[i] * _paulis[i+1]

    if qubit==0:
        Pi1 = np.kron(_s0, Pi1)
        Pi2 = np.kron(_s0, Pi2)
    else:
        Pi1 = np.kron(Pi1, _s0)
        Pi2 = np.kron(Pi2, _s0)

    measurement = [Pi1, Pi2]
    return measurement

def quantum_conditional_entropy(rho, theta, phi, qubit=0):
    """
    The quantum conditional entropy for a two-qubit state,
    with a projective measurement in the specified direction

    Evaluates the formula

    .. math::

        \\sum_k p_k S(\\rho_k^B)

    where p_k is the probability of outcome k in the measurement, `\\rho_k^B` is the
    reduced state of the non-measured qubit after the measurement, and S(\\rho) is the
    von-Neumann entropy (log in base 2).

    Args:
        rho (Array): a two-qubit density operator
        theta (float): "latitude" angle for the direction of the measurement
        phi (float): "longitude" angle for the direction of the measurement
        qubit (int): 0 or 1, the qubit on which the measurement is done (default 0)

    Returns:
        float: the quantum conditional entropy
    """
    measurement = projective_measurement(theta, phi, qubit=qubit)
    prob = np.array([np.real(np.trace(p @ rho)) for p in measurement])
    rho_cond = [partial_trace(p @ rho @ p, [qubit]).data for p in measurement]
    rho_cond = [rho_cond[i] / prob[i] for i in range(len(prob))]
    s_ent = np.array(list(map(entropy, rho_cond)))
    return np.sum(prob * s_ent)

def classical_correlation(rho, qubit=0):
    """
    Calculate the truly classical correlations between two qubits.

    The classical correlations are defined e.g. in Eq. (8)
    of Phys. Rev. A 83, 052108 (2011). We use base 2 for log.

    Args:
        rho (Array): a two-qubit density operator
        qubit (int): 0 or 1, the qubit on which the measurement is done (default 0)

    Returns:
        float: classical correlations
    """
    assert rho.shape == (4, 4), "Not a two-qubit density matrix"
    cc = lambda x: quantum_conditional_entropy(rho, x[0], x[1], qubit=qubit)
    f = minimize(cc, [np.pi/2, np.pi])
    return (entropy(partial_trace(rho, [qubit])) - f.fun)/np.log(2)

def discord(rho, qubit=0):
    """
    The quantum discord between two qubits

    Quantum discord is defined in Phys. Rev. Lett. 88, 017901 (2001).
    We use base 2 for log.

    Args:
        rho (Array): a two-qubit density operator
        qubit (int): 0 or 1, the qubit on which the measurement is done (default 0)

    Return:
        float: quantum discord, between [0, 1]
    """
    return (mutual_information(rho, 2)/np.log(2) - classical_correlation(rho, qubit=qubit))
