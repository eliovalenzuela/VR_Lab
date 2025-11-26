Virual Scenario of Remote Handling Control Laboratory at UGR. Contains 3D models of:
- Staübli TX2-90 robotic arm
- Lifting platform (same dimensions, but not the exact model)
- Conveyor belt (similar but not the same)
- Tool exchanger parking station
- Tools: Zimmer gripper and Rexroth nutrunner

  <img width="1579" height="937" alt="image" src="https://github.com/user-attachments/assets/bec2a73a-b026-48c8-a566-ecca0588eae6" />

The data interface with the physical components is based on OPC UA addon from Unity Assets Store (https://doc.realvirtual.io/). The data requests are configured in pooling mode every 150ms approximately. The refresh rate of the scene deppends on the host HW cappabilities (GPU and ram mostly), and have been tested up the a maximum of 140fps. 
