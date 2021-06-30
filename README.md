# A Car Perception Simulator for Unity

This is a car perception simulator for Unity3D developed as an perception pipeline testing tool for [Einstein Motorsport](https://einstein-motorsport.com/).
It currently supports the simulation of a Lidar sensor as well as a camera and can send this data live to ROS for inference.

## Credit

- The ROS-Connection is done through [Unity's own ROS Integration](https://github.com/Unity-Technologies/Unity-Robotics-Hub) which is largely based on Dr. Martin Bischoffs [ROS-Sharp](https://github.com/MartinBischoff/ros-sharp)
- The Track tool (currently under development) is based on Sebastian Lagues [Path Creator](https://github.com/SebLague/Path-Creator)

## Installation

### Unity Side

- Clone this repo
- Open it in Unity 2021.0+
- Install Unity's [ROS-TCP-Connector](https://github.com/Unity-Technologies/ROS-TCP-Connector)
- Insert the local IP of your machine running ROS into the ROS Connection Gameobject in the hirachy

### ROS Side

- Clone Unity's [ROS-TCP-Endpoint](https://github.com/Unity-Technologies/ROS-TCP-Endpoint) into your catkin Workspace /src folder
- Copy Unitys's [server_endpoint.py](https://github.com/Unity-Technologies/Unity-Robotics-Hub/blob/main/tutorials/ros_packages/robotics_demo/scripts/server_endpoint.py) into your scripts folder and add the Publishers you need for receiving the data
- catkin_make
- add the source command (e.g. "source ~/catkin_ws/devel/setup.bash") to your .bashrc
- start roscore
- edit the config yaml under ROS-TCP-Endpoint/config/params.yaml and insert your local IPv4 address
- load this config file with rosparam load
- rosrun the server_endpoint.py from earlier

### If you run ROS on a VM

Make sure to use a bridged network adapter and allow permiscuous use

## Roadmap/Todo

-[ ] Read Camera pixels in Compute Shader instead of on the CPU for higher performance
-[ ] Maybe call ros.Send() in seperate thread
-[ ] Add noise slider for Lidar
-[ ] Add track tool
-[ ] Add track-follow tool
-[ ] Add models for official Formula Student cones
-[ ] Add rostf state publisher
-[ ] Add groundspeed sensor publisher
-[ ] Make this README better
-[ ] Use Universal Renderpipeline for more realistic graphics
-[ ] Create callbacks for Controler output, so that the perception pipeline output, can be used as inputs for this sim for a feedback loop
