from __future__ import print_function  # allow it to run on python2 and python3
activate_this_file = r"C:\Users\JIA\dwave\Scripts\activate_this.py" #filepath to virtualenvironment, where the dwave libraries are installed
exec(open(activate_this_file).read(), {'__file__': activate_this_file})
import numpy as np
from dwave.system.samplers import DWaveSampler
from dwave.system.composites import EmbeddingComposite
from dwave.cloud import Client

# load matrix
with open('qubomatrix.txt') as f:
    qubo=f.readlines()[0]

#Preprocessing (input to data)
print("\nPreprocess data . . .")
with open('data.txt') as f:
    params = f.readlines()
arguments = params[0]
qaInfo=params[1]
formulation=(qaInfo.split(",")[0]).strip()
solver=(qaInfo.split(",")[1]).strip()
inspector=(qaInfo.split(",")[2]).strip()

#run Problem
print("\nRun problem on DWave in Canada . . .")
sampler = EmbeddingComposite(DWaveSampler(
    solver=solver,
))
client=Client.from_config()#change solver to individual solver
client.close()
responses_arr=[]
if formulation=="qubo":
    exec("responses_arr.append(sampler.sample_qubo("+qubo+", "+arguments+"))")
if formulation=="ising":#                                              isingHDict     isingJDict
    exec("global responses_arr; responses_arr.append(sampler.sample_ising("+qubo+", "+params[4]+", "+arguments+"))")
print("  received response")

print("\nPostprocess responses . . .")
#führe gleiche antworten zusammen
response=responses_arr[0]
if(len(responses_arr)>1):
    print("  merge equal results")
    indices_delete=[]
    for i in range(len(response.record)):
        for j in range(len(response.record)):
            if(response.record[i]["energy"]==response.record[j]["energy"]) and str(response.record[i]["sample"])==str(response.record[j]["sample"]) and not(i in indices_delete[:]) and i!=j :
                response.record[i]["num_occurrences"]+=response.record[j]["num_occurrences"]
                response.record[j]["num_occurrences"]=0
                indices_delete.append(j)
                print("found two same results")

#sortiere nach energie
print("  sort results")
response.record.sort(order=["energy"])

# save results in results.txt
with open('results.txt','w',newline='') as file:
    #file.write('energy\tnum_occurrences\tsample\n')
    for sample, energy, num_occurrences, cbf in response.record:
        file.write('%f\t%g\t%d\t%s\n' % (energy,cbf, num_occurrences, str(sample).replace("\n","")))
    print('Saved results in results.txt')

if inspector == "true":
    print("\nOpen DWave inspector in Browser")
    dwave.inspector.show(response)