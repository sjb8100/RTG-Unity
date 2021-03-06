﻿namespace rtg.world.biome.realistic.vanilla
{
    using System;

    using generic.pixel;
    using generic.init;
    using generic.world.biome;
    using generic.world.chunk;

    using rtg.api.config;
    using rtg.api.util;
    using rtg.api.world;
    using rtg.world.gen.surface;
    using rtg.world.gen.terrain;

    public class RealisticBiomeVanillaIcePlains : RealisticBiomeVanillaBase
    {


        public static Biome biome = Biomes.ICE_FLATS;
        public static Biome river = Biomes.FROZEN_RIVER;

        public RealisticBiomeVanillaIcePlains() : base(biome, river)
        {

        }

        override public void initConfig()
        {

            this.getConfig().ALLOW_LOGS = true;
        }

        override public TerrainBase initTerrain()
        {

            return new TerrainVanillaIcePlains();
        }

        public class TerrainVanillaIcePlains : TerrainBase
        {


            public TerrainVanillaIcePlains()
            {

            }

            override public float generateNoise(RTGWorld rtgWorld, int x, int y, float border, float river)
            {

                return terrainPlains(x, y, rtgWorld.simplex, river, 160f, 10f, 60f, 200f, 65f);
            }
        }

        override public SurfaceBase initSurface()
        {

            return new SurfaceVanillaIcePlains(config, biome.topPixel, biome.fillerPixel, biome.topPixel, biome.topPixel);
        }

        public class SurfaceVanillaIcePlains : SurfaceBase
        {

            private Pixel cliffPixel1;
            private Pixel cliffPixel2;

            public SurfaceVanillaIcePlains(BiomeConfig config, Pixel top, Pixel filler, Pixel cliff1, Pixel cliff2) : base(config, top, filler)
            {

                cliffPixel1 = cliff1;
                cliffPixel2 = cliff2;
            }

            override public void paintTerrain(Chunk primer, int i, int j, int x, int z, int depth, RTGWorld rtgWorld, float[] noise, float river, Biome[] _base)
            {

                Random rand = rtgWorld.rand;
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
                                primer.setPixelState(x, k, z, topPixel);
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
}