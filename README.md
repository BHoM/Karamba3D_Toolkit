# Karamba_Toolkit
## Description
This Toolkit can be used for interoperability between Karamba and BHoM.

## Currently supported conversions and limitations
- Currently, the supported conversion is **from Karamba models to BHoM models**, which can then be exported to other software via Adapters (e.g. Robot_Toolkit, etc.).
- **Only linear elements (i.e. beams)** are currently supported. 
- A variety of materials are supported. Some materials are converted to custom materials.
- Load cases, combinations, non-surface loads and support conditions (Constraint6DOF) are supported.

This functionalty already allows to leverage Karamba and BHoM to a good extent. For example, a structural engineering model can be created via Karamba, then converted to BHoM and finally exported to external FEA software via the extensive suite of BHoM Adapters.

The conversion from BHoM models to Karamba is not yet implemented.

## Set up

### :information_source: NOTE: Requires installation of Karamba V3 (pre-release) :information_source:
Install from:
https://github.com/karamba3d/K3D_NightlyBuilds/releases/tag/3.0.0.4-WIP

Make sure you have the right version by using the Karamba License component:

<img src="https://user-images.githubusercontent.com/6352844/221171386-ed6ee839-b0d5-4188-9b8b-c8655953a0b7.png" width="500"/>


### Requires BHoM (latest version)
Install the latest beta from https://bhom.xyz/.

### Requires assemblies of Karamba_Toolkit in the BHoM folder
Clone and compile the Karamba_Toolkit to get them. Make sure Rhino/Excel/Revit are closed before compiling.

## Example usage

0. Perform set up (see above)
1. Create a Karamba model. See [official examples](https://karamba3d.com/learn/examples/). Remember the limitations listed above.
2. In Grasshopper, use the BHoM search menu to find the Karamba_Toolkit method called `ToBHoM`. To do this, press `CTRL+Shift+B` and type `Karamba tobhom`.
3. Take the assembled Karamba model and feed it as an input to the `ToBHoM` component. It will output a BHoM `FemModel` object.
4. Push the converted `FemModel` to an external structural software of choice. For instructions on this, see [Introduction to BHoM_Adapter](https://bhom.xyz/documentation/BHoM_Adapter/#example-usage-use-robot-adapter-to-push-export-a-bhom-model-to-robot). It takes 5 minutes to read and set up a working example.

### Sample file: Beam cantilever
Example file: [Beam_Cantilever.zip](https://github.com/BHoM/Karamba3D_Toolkit/files/10824421/Beam_Cantilever.zip)