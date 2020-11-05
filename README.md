### 'Punched cards' proof of concept

*Object recognition by random binary data lookup: proof of concept*

Performing single-shot QMNIST digits recognition by lookup over the most different input bit sets of the training data.

Program output:
```
Punched card bit length: 32

Global top punched card:
Unique input combinations per punched card (descending): {5090, 4931, 4917, 4782, 4401, 4297, 4041, 4040, 3989, 807: sum 41295}: total sum 41295
Training results: 25585 correct recognitions of 60000
Test results: 25568 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {4401, 4297, 4041: sum 12739}, {5376, 5355: sum 10731}, {5470}, {4861}, {4740}, {4586}, {1595}: total sum 44722
Training results: 17834 correct recognitions of 60000
Test results: 18182 correct recognitions of 60000

Punched card bit length: 64

Global top punched card:
Unique input combinations per punched card (descending): {6120, 6119, 5946, 5911, 5903, 5842, 5830, 5744, 5373, 2726: sum 55514}: total sum 55514
Training results: 29824 correct recognitions of 60000
Test results: 29901 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6120, 6119, 5946, 5903, 5842, 5744: sum 35674}, {5914}, {5870}, {5379}, {3193}: total sum 56030
Training results: 22331 correct recognitions of 60000
Test results: 22669 correct recognitions of 60000

Punched card bit length: 128

Global top punched card:
Unique input combinations per punched card (descending): {6482, 6264, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 59739}: total sum 59739
Training results: 32014 correct recognitions of 60000
Test results: 32049 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6131, 5958, 5923, 5918, 5851: sum 29781}, {5949, 5842: sum 11791}, {6482}, {6265}, {5421}: total sum 59740
Training results: 28366 correct recognitions of 60000
Test results: 28621 correct recognitions of 60000

Punched card bit length: 256

Global top punched card:
Unique input combinations per punched card (descending): {6742, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 60000}: total sum 60000
Training results: 36521 correct recognitions of 60000
Test results: 36892 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6742, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 60000}: total sum 60000
Training results: 36521 correct recognitions of 60000
Test results: 36890 correct recognitions of 60000

Punched card bit length: 512

Global top punched card:
Unique input combinations per punched card (descending): {6742, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 60000}: total sum 60000
Training results: 41038 correct recognitions of 60000
Test results: 41105 correct recognitions of 60000

Top punched cards per label:
Unique input combinations per punched card (descending): {6742, 6265, 6131, 5958, 5949, 5923, 5918, 5851, 5842, 5421: sum 60000}: total sum 60000
Training results: 41038 correct recognitions of 60000
Test results: 41105 correct recognitions of 60000

Press "Enter" to exit the program...
```

### Interesting results:
* Recognition accuracy on a best 512 bit punched card is 68.5%
* One (global top) punched card works better or equal than set of top punched cards per specific label on any bit length
* Starting from bit length 128 there are cases when all the inputs per specific label of the punched card are unique
* Test set accuracy increasing linearly with the doubling of the punched card bit length

### Questions to answer:
* How to rank the punched cards when all the inputs per specific label are unique?
* How to make sure that chosen punched card is the best? Need to evaluate all of them?

### Ideas:
* It's possible to calculate the entropy of the inputs of the specific punched card per label (to rank them)
* Try calculate the relative importance of each bit of the punched card input (like attention mechanism). Use this metric for ranking.
* Build hierarchy of punched cards to perform lookup over the multiple punched card results intead of simple max
* Prefer processing with less active bits on all stages (since electrical pulses in a brain requires energy)
* Encode (or map) the binary input somehow to have/keep the 2% sparsity (like in a brain)
