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

    public class SurfaceGrasslandMix1 : SurfaceBase
    {

        private Pixel mixPixel;
        private Pixel cliffPixel1;
        private Pixel cliffPixel2;
        private float width;
        private float height;

        public SurfaceGrasslandMix1(BiomeConfig config, Pixel top, Pixel filler, Pixel mix, Pixel cliff1, Pixel cliff2, float mixWidth, float mixHeight) : base(config, top, filler)
        {
            mixPixel = mix;
            cliffPixel1 = cliff1;
            cliffPixel2 = cliff2;

            width = mixWidth;
            height = mixHeight;
        }

        override public void paintTerrain(Chunk primer, int i, int j, int x, int z, int depth, RTGWorld rtgWorld, float[] noise, float river, Biome[] _base)
        {

            Random rand = rtgWorld.rand;
            OpenSimplexNoise simplex = rtgWorld.simplex;
            float c = CliffCalculator.calc(x, z, noise);
            bool cliff = c > 1.4f ? true : false;

            for (int k = 255; k > -1; k--)
            {
                Pixel b = primer.getPixelState(x, k, z).getPixel();
                if (b == Pixels.AIR)
                {
                    depth = -1;
                }
                else if (b == Pixels.STONE)
                {
                    depth++;

                    if (cliff)
                    {
                        if (depth > -1 && depth < 2)
                        {
                            primer.setPixelState(x, k, z, rand.Next(3) == 0 ? cliffPixel2 : cliffPixel1);
                        }
                        else if (depth < 10)
                        {
                            primer.setPixelState(x, k, z, cliffPixel1);
                        }
                    }
                    else
                    {
                        if (depth == 0 && k > 61)
                        {
                            if (simplex.noise2(i / width, j / width) > height) // > 0.27f, i / 12f
                            {
                                primer.setPixelState(x, k, z, mixPixel);
                            }
                            else
                            {
                                primer.setPixelState(x, k, z, topPixel);
                            }
                        }
                        else if (depth < 4)
                        {
                            primer.setPixelState(x, k, z, fillerPixel);
                        }
                    }
                }
            }
        }
    }
}