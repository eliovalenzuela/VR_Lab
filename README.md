Virual Scenario of Remote Handling Control Laboratory at UGR. Contains 3D models of:
- Staübli TX2-90 robotic arm
- Lifting platform (same dimensions, but not the exact model)
- Conveyor belt (similar but not the same)
- Tool exchanger parking station
- Tools: Zimmer gripper and Rexroth nutrunner

  <img width="1579" height="937" alt="image" src="https://github.com/user-attachments/assets/bec2a73a-b026-48c8-a566-ecca0588eae6" />

The data interface with the physical components is based on OPC UA addon from Unity Assets Store (https://doc.realvirtual.io/). The data requests are configured in pooling mode every 150ms approximately. The refresh rate of the scene depends on the host HW capabilities (GPU and RAM mostly), and has been tested up to a maximum of 140fps. 

Input trajectories in 3D space are introduced by the user using a Touch device from 3D Systems. The libraries used to manage the device are the official ones provided by the Open Haptics repo (https://www.3dsystems.com/haptics-devices/openhaptics).
<img width="940" height="494" alt="image" src="https://github.com/user-attachments/assets/a613ff1d-0859-40ed-86d3-c5bed9d571d1" />
