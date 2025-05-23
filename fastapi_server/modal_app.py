import io
import soundfile as sf
from fastapi import FastAPI, Request, HTTPException
from transformers import pipeline
import numpy as np
import modal

app = modal.App("whisper-fastapi")

image = (
    modal.Image.debian_slim()
    .pip_install("transformers", "torch", "soundfile", "torchaudio", "fastapi", "uvicorn[standard]", )
)

with image.imports():
    import torch

@app.function(image=image, gpu="any", max_containers=2)
@modal.asgi_app()
def fastapi_app():
    app = FastAPI()
    asr_pipeline = pipeline("automatic-speech-recognition", model="openai/whisper-small", device=0)
 


    @app.post("/transcribe")
    async def transcribe(request: Request):
        data = await request.body()
        try:
            audio_np, sr = sf.read(io.BytesIO(data))
            if audio_np.ndim > 1:
                audio_np = audio_np.mean(axis=1)
            result = asr_pipeline({"array": audio_np, "sampling_rate": sr})
            return {"text": result["text"]}
        except Exception as e:
            raise HTTPException(status_code=500, detail=str(e))

    return app
