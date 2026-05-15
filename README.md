# Welcome to the crowd simulator! 👩‍💻
![image](https://github.com/user-attachments/assets/e0cd43e8-44e3-449a-8142-bb1b5b03dfd9)

👩‍💻 Please fork this repo if you plan to create a project based on this version.

## General information
- Verified to work with Unity 2022.3.18f1.
- There is a readme for the components of the simulator: [`/Assets/CrowdSimulator/README.md`](https://github.com/KTH-High-Density-Crowd-Simulator/Crowd-Simulator-and-Tbana-Reconstruction/blob/main/Crowd%20Simulator%20and%20Tbana%20Reconstruction/Assets/CrowdSimulator/README.md).
- There is a template scene, which you can use as a basis for your own project, `/Assets/CrowdSimulator/Scenes/Template.unity`
- There are sample scenes in `/Assets/CrowdSimulator/Scenes`.
- Prefabs needed for the simulation can be found in `/Assets/CrowdSimulator/Prefabs`.
- Please use the agent models in `/Assets/CrowdSimulator/Prefabs/Agents`.
- The models are (almost) scaled to match real life (1 unit in Unity = 1 meter).
- The subway reconstruction assets and scene can be found in `/Assets/Metroped`
- The sound assets exists in the folder `/Assets/Soundscape`
- Under `/Assets/Metroped/Scripts` code files for the Digital signs, the functionality of the metro trains, the sound and movements of the player as well as tools for demo and UI scripts can be found

## How to download and use
1. Download Unity 2022.3.18f1
2. Clone this repository
3. Open the MetroPed "Crowd Simulation and Tbana Reconstruction" folder using the Unity launcher
4. Once the project is loaded in click the "Play" button and choose the desired settings (VR/Regular, with or without scenarios)
5. With this the simulation should be runnable and operational

## Unity Hiearchy
- StationExtended: contains all the 3D models for the metro station including the subway trains and their functionality.
    - All soundscape functionality, AudioSources, and AudioListeners can be found under the parent object. Ex. AudioSource for the subway train is under that Subway object.
- FPS Overlay: contains and FPS overlay to display the performance of the simulation at runtime.
- XR_VR_Movement: contains all assets in relation to the player character and its movement in VR or using PC controls.
- CrowdSimulation: Contains all code and assets in relation to the crowd simulator, its agents, scripts, and functionality.
- BarriersYellowLine: contains the assets connected to the invisible barrier at the yellow line to stop the player from crossing
- UI: contains all assets in regards to the User Interface 
- GameManager: currently on has assets for chaning exposure in the metro station
- ScenarioManager: contains scripts for toggling the crowd as well as manipulating scenario setings such as exposure as weell as toggling between different layouts (glass panels, walls, pillars and so on)
- Spawner: is separated into left, right and one for each side of the station. They contain scripts for spawning the agents into the metro. 


## Acknowledgements
### Jack Shabo

- Original implementation of high density crowd simulator.
- [High Density Simulation of Crowds with Groups in Real-Time](https://urn.kb.se/resolve?urn=urn:nbn:se:kth:diva-210564)

### PVK Software Engineering group and Julian Ley

- Created subway reconstruction.
- [Metroped Interactive](https://github.com/JulianLey/MetropedInteractive)

### Jenna Smulter
- Improvements and bug fixes to crowd simulator. Owner of this repo.

### Anton Porsbjer
- Improvements and bug fixes to crowd simulator. Owner of this repo.

## Screenshots

![platform](https://github.com/user-attachments/assets/9f0915a1-38f2-43c5-8779-f3f2bc1480b0)

![mycketfolk](https://github.com/user-attachments/assets/513d1935-565c-4e22-adf2-e14af7e557df)



