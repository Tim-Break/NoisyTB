# NoiseTB
This is my simple library of two‑dimensional noises.
<br>
<img width="370" height="370" alt="Noises" src="https://github.com/user-attachments/assets/36eae9b6-521c-4350-b110-f651fd255a8e" />
## Noises
### 1. White Noise
<img width="370" height="370" alt="White noise" src="https://github.com/user-attachments/assets/012f93ac-1440-48ef-8e5e-61b1b97809c7" />
<br>
The following code* was used to generate this image:

```csharp
for (int x = 0; x < 1000; x++)
{
    for (int y = 0; y < 1000; y++)
    {
        float value = Noise.To01(Noise.White(x, y, 42));
        // byte pixelValue = (byte)(value * 255);
    }
}
```
<br>

### Gradient Noise
<img width="370" height="370" alt="Gradient noise" src="https://github.com/user-attachments/assets/f41e37a0-d40e-4107-94ea-941597a3b295" />
<br>
The following code* was used to generate this image:

```csharp
for (int x = 0; x < 1000; x++)
{
    for (int y = 0; y < 1000; y++)
    {
        float value = Noise.To01(Noise.Gradient(new Vector2(x / 100f, y / 100f), 42));
        // byte pixelValue = (byte)(value * 255);
    }
}
```
<br>

### Warped Gradient Noise
<br>

### Jittered Gradient Noise
<br>

### FBM Gradient Noise
<br>

### FBM Jittered Gradient Noise
<br>

### Voronoi Noise
<br>
