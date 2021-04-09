""" 
Visualization of pairwise relations
"""

import networkx as nx
import matplotlib.pyplot as plt
import numpy as np
import itertools

def make_graph(ent_dict, qubit_list):
    G = nx.Graph()
    G.add_nodes_from(qubit_list)
    for i in range(len(qubit_list)-1):
        qi = qubit_list[i]
        for j in range(i+1,len(qubit_list)):
            qj = qubit_list[j]
            if ent_dict[(qi,qj)] != 0.0:
                G.add_edge(qi,qj,weight=ent_dict[(i,j)])
    return G

def draw_entanglement_graph(ent_dict, qubit_list=None, layout="circular", scale_factor = 1., labels = {}, node_color = "#0A7290", **kwargs):
    if not qubit_list:
        qubit_list = list(set(itertools.chain.from_iterable(ent_dict.keys())))
    
    G = make_graph(ent_dict, qubit_list)

    if layout == "circular":
        pos = nx.circular_layout(G, dim=2, scale=1, center=None)
    elif layout == "spring":
        pos = nx.spring_layout(G)
    else:
        raise ValueError("Not a valid layout name (circular, spring).")

    label_pos = [pos[i]*1.3 for i in pos]    
    
    edgewidth = [d['weight']*10*scale_factor for (u,v,d) in G.edges(data=True)]
    nodesize = [(e[1]*200+10)*scale_factor for e in G.degree(weight='weight')]
    
    nx.draw_networkx_nodes(G, pos, node_size=nodesize, node_color=node_color, edgecolors="k", linewidths=0.3, **kwargs)
    nx.draw_networkx_edges(G, pos, width=edgewidth, **kwargs)
    nx.draw_networkx_labels(G, label_pos, labels=labels, font_size=10, **kwargs)
    
