# cortical-extractor

This project implements semiautomatic extraction of cortical bone from sets of CT scans. It is possible to replicate the segmentation and medial axis detection of [1] and [2] with this tool. Other steps like placing landmarks, 1D registration of extracted profiles from [1], PCA and graphing need to be performed using other tools.

Additional information can be found in [doc](doc/howto/howto.pdf).

## Requirements
This project uses .NET 8.0 and can be built using Visual Studio, without additional tools.

## References
[1] Dupej J, Lacoste Jeanson A, Pelikán J, Brůžek J. Semiautomatic extraction of cortical thickness and diaphyseal curvature from CT scans. Am J Phys Anthropol. 2017; 164: 868–876. https://doi.org/10.1002/ajpa.23315

[2] Bondioli L, Bayle P, Dean C, Mazurier A, Puymerail L, Ruff C, Stock JT, Volpato V, Zanolli C, Macchiarelli R. Technical note: Morphometric maps of long bone shafts and dental roots for imaging topographic thickness variation. Am J Phys Anthropol. 2010; 142:328-34.https://doi.org/10.1002/ajpa.21271