﻿namespace rtg.world.gen.surface
{
    using System;

    using generic.pixel;
    using generic.init;
    using generic.world.biome;
    using generic.world.chunk;

    using rtg.api.util.noise;
    using rtg.api.world;
    using rtg.api.config;
    using rtg.api.util;

    public class SurfaceMountainSnow : SurfaceBase
    {

        private float min;

        private float sCliff = 1.5f;
        private float sHeight = 60f;
        private float sStrength = 65f;
        private float iCliff = 0.3f;
        private float iHeight = 100f;
        private float iStrength = 50f;
        private float cCliff = 1.5f;

        public SurfaceMountainSnow(BiomeConfig config, Pixel top, Pixel fill, float minCliff) : base(config, top, fill)
        {

            min = minCliff;
        }

        public SurfaceMountainSnow(BiomeConfig config, Pixel top, Pixel fill, float minCliff, float stoneCliff, float stoneHeight, float stoneStrength, float snowCliff, float snowHeight, float snowStrength, float clayCliff) : this(config, top, fill, minCliff)
        {

            sCliff = stoneCliff;
            sHeight = stoneHeight;
            sStrength = stoneStrength;
            iCliff = snowCliff;
            iHeight = snowHeight;
            iStrength = snowStrength;
            cCliff = clayCliff;
        }

        override public void paintTerrain(Chunk primer, int i, int j, int x, int z, int depth, RTGWorld rtgWorld, float[] noise, float river, Biome[] _base)
        {

            Random rand = rtgWorld.rand;
            OpenSimplexNoise simplex = rtgWorld.simplex;
            float c = CliffCalculator.calc(x, z, noise);
            int cliff = 0;

            Pixel b;
            for (int k = 255; k > -1; k--)
            {
                b = primer.getPixelState(x, k, z).getPixel();
                if (b == Pixels.AIR)
                {
                    depth = -1;
                }
                else if (b == Pixels.STONE)
                {
                    depth++;

                    if (depth == 0)
                    {

                        float p = simplex.noise3(i / 8f, j / 8f, k / 8f) * 0.5f;
                        if (c > min && c > sCliff - ((k - sHeight) / sStrength) + p)
                        {
                            cliff = 1;
                        }
                        if (c > cCliff)
                        {
                            cliff = 2;
                        }
                        if (k > 110 + (p * 4) && c < iCliff + ((k - iHeight) / iStrength) + p)
                        {
                            cliff = 3;
                        }

                        if (cliff == 1)
                        {
                            if (rand.Next(3) == 0)
                            {

                                primer.setPixelState(x, k, z, hcCobble(rtgWorld, i, j, x, z, k));
                            }
                            else
                            {

                                primer.setPixelState(x, k, z, hcStone(rtgWorld, i, j, x, z, k));
                            }
                        }
                        else if (cliff == 2)
                        {
                            primer.setPixelState(x, k, z, getShadowStonePixel(rtgWorld, i, j, x, z, k));
                        }
                        else if (cliff == 3)
                        {
                            primer.setPixelState(x, k, z, Pixels.SNOW);
                        }
                        else if (k < 63)
                        {
                            if (k < 62)
                            {
                                primer.setPixelState(x, k, z, fillerPixel);
                            }
                            else
                            {
                                primer.setPixelState(x, k, z, topPixel);
                            }
                        }
                        else
                        {
                            primer.setPixelState(x, k, z, Pixels.GRASS);
                        }
                    }
                    else if (depth < 6)
                    {
                        if (cliff == 1)
                        {
                            primer.setPixelState(x, k, z, hcStone(rtgWorld, i, j, x, z, k));
                        }
                        else if (cliff == 2)
                        {
                            primer.setPixelState(x, k, z, getShadowStonePixel(rtgWorld, i, j, x, z, k));
                        }
                        else if (cliff == 3)
                        {
                            primer.setPixelState(x, k, z, Pixels.SNOW);
                        }
                        else
                        {
                            primer.setPixelState(x, k, z, Pixels.DIRT);
                        }
                    }
                }
            }
        }
    }
}