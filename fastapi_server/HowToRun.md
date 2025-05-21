# Run The local fastapi Server
We are utilizing ngrok to have our local server as a hosting server. We choose ngrok appraoch because of 
- Can't modify the firewall rules of school laptop
- eventually had to either use cloud or have our laptop as a server

Running the server and ngrok
- Using terminal,  in 'fastapi_server' directory run the following command
    - uvicorn main:app --host 0.0.0.0 --port 5000
- Using terminal, change the directory where the 'ngrok.exe. file is and run the following command:
    - .\ngrok.exe http 5000

Testing the server
- Testing of server can be done by running 'testLocal.py' file


# AirLink Connection
- Make sure both the laptop and meta quest have the same network (I usually use an hotspot from my phone, also make sure the bandwidth is 5Hz not 2.5Hz)
- Start the meta quest link app and in the vr enable airlink and launch the application
- in the unity editor, under edit/XR Plug-in Management, make sure to selection PC build not the andriod build
- Had to manually enable XR Runtime setting by going into the windows registry, which enabled to see the app in the meta quest #3
- 'ngrok.exe' is now listed as an potential virus, so no longer can be use ngork.exe unless we ask the admin to fix it