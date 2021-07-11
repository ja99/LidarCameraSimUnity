# A Car Perception Simulator for Unity

This is a car perception simulator for Unity3D developed as an perception pipeline testing tool for [Einstein Motorsport](https://einstein-motorsport.com/).
It currently supports the simulation of a Lidar sensor as well as a camera and can send this data live to ROS for inference.

Scene view:

![scene_view](https://imgshare.org/i/azZG3596.png)

Scene view + perception Debug:

![scene_view_perception_debug](https://imgshare.org/i/mds33726.jpg)


RVIZ view:

![rviz_view](https://imgshare.org/i/tTX13595.jpg)


## Credit

- The ROS-Connection is done through [Unity's own ROS Integration](https://github.com/Unity-Technologies/Unity-Robotics-Hub) which is largely based on Dr. Martin Bischoffs [ROS-Sharp](https://github.com/MartinBischoff/ros-sharp)
- The Track tool is based on Sebastian Lagues [Path Creator](https://github.com/SebLague/Path-Creator)
- The cone models were created by [Torsten Hahn](https://github.com/RoostOne)
## Installation

### Unity Side

- Clone this repo
- Open it in Unity 2021.0+
- Install Unity's [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector)
- Insert the local IP of your machine running ROS into the ROS Connection Gameobject in the hirarchy
- Download Newsofts Json tool from here https://github.com/jilleJr/Newtonsoft.Json-for-Unity and install it as well (Unitys own Json Utility doesn´t work reliably for me)

### ROS Side

- Clone Unity's [ROS-TCP-Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint) into your catkin Workspace /src folder
- Copy Unitys's [server_endpoint.py](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/ros_packages/robotics_demo/scripts/server_endpoint.py) into your scripts folder and make two duplicates of it
- Add an image raw publisher to the first and give it the port 10001
- Add an pointcloud publisher to the first and give it the port 10002
- catkin_make
- add the source command (e.g. "source ~/catkin_ws/devel/setup.bash") to your .bashrc
- start roscore
- edit the config yaml under ROS-TCP-Endpoint/config/params.yaml and insert your local IPv4 address
- load this config file with rosparam load
- rosrun the two server_endpoints from earlier

### If you run ROS on a VM

Make sure to use a bridged network adapter and allow permiscuous use

## Roadmap/Todo

- [X] Read Camera pixels in Compute Shader instead of on the CPU for higher performance
- [X] Maybe call ros.Send() in seperate thread (improved performance from about 5hz to 10hz)
- [X] Try multiple simultaneous connections (improved performance from 10 hz to 15hz for the lidar and 25 hz for the camera)
- [X] Add track tool
- [X] Add track-follow tool
- [X] Add models for official Formula Student cones (Thanks to Torsten Hahn)
- [X] Create callbacks for Controler output, so that the perception pipeline output, can be used as inputs for this sim for a feedback loop
- [X] Add groundspeed sensor publisher (Added CarState Publisher)
- [ ] Try interpreting the uint Array coming from the compute shader as byte Array to get rid of the loop.
- [ ] Add noise slider for Lidar
- [ ] Add rostf state publisher
- [ ] Make this README better
- [ ] Use Universal Renderpipeline for more realistic graphics
- [ ] implement realistic car physics
