using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    //fractal brownian motion function
    //takes x,y of height map and returns x,y transformed with fBM
    //returns value between 0 and 1
      public static float fBM(float x, float y, int oct, float persistence, float frequencyMult) {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;
        
        for (int i=0; i <oct; i++) {
            total += Mathf.PerlinNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= frequencyMult;
        }

        return total / maxValue;
    }

    public static float Map(float value, float originalMin,
        float originalMax, float targetMin, float targetMax) {
        return (value - originalMin) * (targetMax - targetMin) / 
            (originalMax - originalMin) + targetMin;
    }

    //fisher-yates shuffle
    public static System.Random r = new System.Random();
    public static void Shuffle<T>(this IList <T> list) {
        int n = list.Count;
        while(n > 1) {
            n--;
            int k = r.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    //gets neighbours from 2d matrix, 6 for edge case or 9 for normal 
    public static List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height) {
        List<Vector2> neighbours = new List<Vector2>();
        //loops from -1 to 1 on both x and y
        for (int y = -1; y < 2; y++) {
            for (int x = -1; x < 2; x++) {
                if (!(x == 0 && y == 0)) { //dont include own self
                    //validity check to ignore edge cases giving false values
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos)) {
                        neighbours.Add(nPos);
                    }
                }
            }
        }
        return neighbours;
    }

    //gets neighbours from 2d matrix, 6 for edge case or 9 for normal 
    public static List<Vector2> GenerateNeighbours_M(Vector2 pos, int width, int height, int increment) {
        List<Vector2> neighbours = new List<Vector2>();
        //loops from -1 to 1 on both x and y
        for (int y = -1*increment; y < 2*increment; y+=increment) {
            for (int x = -1*increment; x < 2*increment; x+=increment) {
                if (!(x == 0 && y == 0)) { //dont include own self
                    //validity check to ignore edge cases giving false values
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos)) {
                        neighbours.Add(nPos);
                    }
                }
            }
        }
        return neighbours;
    }

    //greatest common divisor
    private static int GCD(int a, int b) {
        while (a != 0 && b != 0) {
            if (a > b) {
                a %= b;
            }
            else {
                b %= a;
            }
        }

        return a == 0 ? b : a;
    }

}
