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