# NoiseTB
This is my simple library of two‑dimensional noises and world heightmap generator.
<br>
<img width="370" height="370" alt="Noises" src="https://github.com/user-attachments/assets/e8d7c77a-695d-4b44-beab-472479adabb3" />
## Noises
### 1. White Noise
<img width="370" height="370" alt="White noise" src="https://github.com/user-attachments/assets/012f93ac-1440-48ef-8e5e-61b1b97809c7" />
<br>
The following code<sup>1</sup> was used to generate this image:

```csharp
for (int x = 0; x < 1000; x++)
{
    for (int y = 0; y < 1000; y++)
    {
        float value = MathUtilities.To01(Noise.White(x, y, 42));
        // byte pixelValue = (byte)(MathUtilities.FClamp01(value) * 255);
    }
}
```
<br>

### Gradient Noise
<img width="370" height="370" alt="Gradient noise" src="https://github.com/user-attachments/assets/f41e37a0-d40e-4107-94ea-941597a3b295" />
<br>
The following code<sup>1</sup> was used to generate this image:

```csharp
for (int x = 0; x < 1000; x++)
{
    for (int y = 0; y < 1000; y++)
    {
        float value = MathUtilities.To01(Noise.Gradient(new Vector2(x / 100f, y / 100f), 42));
        // byte pixelValue = (byte)(MathUtilities.FClamp01(value) * 255);
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

<sup>1</sup>This code shows how to obtain the noise value at a certain point, but it doesn’t show how to generate an image.
