using System;
using System.Collections.Generic;
using System.IO;
using MathUtilities;
using RenderDataStructures.Cameras;
using RenderDataStructures.Materials;
using RenderDataStructures.Materials.Textures;
using RenderDataStructures.Shapes;
using StbSharp;
using Math = System.Math;

namespace RenderHandler
{
    internal class SceneGenerator
    {
        public SceneGenerator(IHitTarget p_world, ICamera p_camera)
        {
            World = p_world;
            Camera = p_camera;
        }

        public ICamera Camera { get; set; }
        public IHitTarget World { get; set; }

        internal static SceneGenerator GenerateThreeSphereScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(0, 0, 0);
            var lookAt = new Vec3(0, 0, -1);
            const double focalDistance = 2;
            const double aperture = 0.0;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 90, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);


            var worldList = new List<IHitTarget>();

            worldList.Add(new Sphere(new Vec3(0, 0, -1), 0.5, new Lambertian(new ConstantTexture(new Color(0.8, 0.3, 0.3)))));
            worldList.Add(new Sphere(new Vec3(0, -100.5, -1), 100, new Lambertian(new ConstantTexture(new Color(0.8, 0.8, 0)))));
            worldList.Add(new Sphere(new Vec3(1, 0, -1), 0.5, new Glossy(new ConstantTexture(new Color(0.8, 0.6, 0.2)), 1.0)));
            worldList.Add(new Sphere(new Vec3(-1, 0, -1), 0.5, new Dielectric(new Color(1.0, 1.0, 1.0), 1.5)));
            worldList.Add(new Sphere(new Vec3(-1, 0, -1), -0.45, new Dielectric(new Color(1.0, 1.0, 1.0), 1.5)));

            var newWorld = new World(worldList);

            var newScene = new SceneGenerator(newWorld, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateTwoLargeSphereNoiseTestBvhScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);

            var returnList = new List<IHitTarget>();

            var perlinTexture = new NoiseTexture(new Color(0.8, 0.8, 0.8), 4.0, NoiseTypes.MARBLE, 0.5, 10, 15);

            returnList.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));

            returnList.Add(new Sphere(new Vec3(0, 1, 0), 1.0, new Lambertian(perlinTexture)));

            return new SceneGenerator(new BvhNode(returnList, 0, 1), camera);
        }

        internal static SceneGenerator GenerateSingleMovingSphereTestScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(0, 0, 1);
            var lookAt = new Vec3(0, 0, -1);
            const double focalDistance = 2;
            const double aperture = 0.0;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 90, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);


            var worldList = new List<IHitTarget>();

            worldList.Add(new MovingSphere(new Vec3(-0.5, 0, -1.0), new Vec3(0.5, 0, -1.0), 0.0, 1.0, 0.35, new Lambertian(new ConstantTexture(new Color(0.8, 0.1, 0.1)))));
            worldList.Add(new Sphere(new Vec3(0, -100.5, -1), 100, new Lambertian(new ConstantTexture(new Color(0.8, 0.8, 0)))));

            var newWorld = new World(worldList);

            var newScene = new SceneGenerator(newWorld, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateRandomScene(RenderParameters p_renderParameters, int p_numberOfObjects)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);


            var rng = new Random();

            var worldList = new List<IHitTarget>
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(new ConstantTexture(new Color(0.5, 0.5, 0.5))))
            };

            for ( var i = -11; i < 11; ++i )
            {
                for ( var j = -11; j < 11; ++j )
                {
                    var materialPicker = rng.NextDouble();
                    var center = new Vec3(i + 0.9 * rng.NextDouble(), 0.2, j + 0.9 * rng.NextDouble());
                    if ( !((center - new Vec3(4, 0.2, 0)).GetLength() > 0.9) ) continue;
                    if ( materialPicker < 0.7 )
                    {
                        worldList.Add(new Sphere(center, 0.2, new Lambertian(new ConstantTexture(new Color(rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble()))) ));
                    }
                    else if ( materialPicker < 0.8 )
                    {
                        worldList.Add(new Sphere(center, 0.2, new Glossy(new ConstantTexture(new Color(0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()))), 0.5 * rng.NextDouble())));
                    }
                    else
                    {
                        worldList.Add(new Sphere(center, 0.2, new Dielectric(new Color(1, 1, 1), 1.5) ));
                    }
                }
            }

            worldList.Add(new Sphere(new Vec3(0, 1, 0), 1.0, new Dielectric(new Color(1, 1, 1), 1.5) ));
            worldList.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new ConstantTexture(new Color(0.4, 0.2, 0.1))) ));
            worldList.Add(new Sphere(new Vec3(4, 1, 0), 1.0, new Glossy(new ConstantTexture(new Color(0.7, 0.6, 0.5)), 0.05)) );

            var newWorld = new World(worldList);

            var newScene = new SceneGenerator(newWorld, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateRandomMovingScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);

            var rng = new Random();

            var checkerTexture = new CheckerTexture(new ConstantTexture(new Color(0.2, 0.3, 0.1)), new ConstantTexture(new Color(0.9, 0.9, 0.9)));

            var worldList = new List<IHitTarget>
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checkerTexture))
            };

            for (var i = -11; i < 11; ++i)
            {
                for (var j = -11; j < 11; ++j)
                {
                    var materialPicker = rng.NextDouble();
                    var center = new Vec3(i + 0.9 * rng.NextDouble(), 0.2, j + 0.9 * rng.NextDouble());
                    if (!((center - new Vec3(4, 0.2, 0)).GetLength() > 0.9)) continue;
                    if (materialPicker < 0.7)
                    {
                        worldList.Add(new MovingSphere(center, center + new Vec3(0, 0.5 * rng.NextDouble(), 0), 0.0, 1.0,  0.2, new Lambertian(new ConstantTexture(new Color(rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble())))));
                    }
                    else if (materialPicker < 0.8)
                    {
                        worldList.Add(new Sphere(center, 0.2, new Glossy(new ConstantTexture(new Color(0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()))), 0.5 * rng.NextDouble())));
                    }
                    else
                    {
                        worldList.Add(new Sphere(center, 0.2, new Dielectric(new Color(1, 1, 1), 1.5)));
                    }
                }
            }

            worldList.Add(new Sphere(new Vec3(0, 1, 0), 1.0, new Dielectric(new Color(1, 1, 1), 1.5)));
            worldList.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new ConstantTexture(new Color(0.4, 0.2, 0.1)))));
            worldList.Add(new Sphere(new Vec3(4, 1, 0), 1.0, new Glossy(new ConstantTexture(new Color(0.7, 0.6, 0.5)), 0.05)));

            var newWorld = new World(worldList);

            var newScene = new SceneGenerator(newWorld, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateRandomMovingBvhScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);

            var rng = new Random();

            var returnList = new List<IHitTarget>();

            var checkerTexture = new CheckerTexture(new ConstantTexture(new Color(0.2, 0.3, 0.1)), new ConstantTexture(new Color(0.9, 0.9, 0.9)) );

            returnList.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(checkerTexture)));
            for (var i = -11; i < 11; ++i)
            {
                for (var j = -11; j < 11; ++j)
                {
                    var materialPicker = rng.NextDouble();
                    var center = new Vec3(i + 0.9 * rng.NextDouble(), 0.2, j + 0.9 * rng.NextDouble());
                    if (!((center - new Vec3(4, 0.2, 0)).GetLength() > 0.9)) continue;
                    if (materialPicker < 0.7)
                    {
                        returnList.Add(new MovingSphere(center, center + new Vec3(0, 0.5 * rng.NextDouble(), 0), 0.0, 1.0, 0.2, new Lambertian(new ConstantTexture(new Color(rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble())))));
                    }
                    else if (materialPicker < 0.8)
                    {
                        returnList.Add(new Sphere(center, 0.2, new Glossy(new ConstantTexture(new Color(0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()))), 0.5 * rng.NextDouble())));
                    }
                    else
                    {
                        returnList.Add(new Sphere(center, 0.2, new Dielectric(new Color(1, 1, 1), 1.5)));
                    }
                }
            }

            returnList.Add(new Sphere(new Vec3(0, 1, 0), 1.0, new Dielectric(new Color(1, 1, 1), 1.5)));
            returnList.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new ConstantTexture(new Color(0.4, 0.2, 0.1)))));
            returnList.Add(new Sphere(new Vec3(4, 1, 0), 1.0, new Glossy(new ConstantTexture(new Color(0.7, 0.6, 0.5)), 0.05)));

            var newWorld = new BvhNode(returnList, 0.0, 1.0);

            var newScene = new SceneGenerator(newWorld, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateEarthBvhScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance, 0.0, 1.0);

            var returnList = new List<IHitTarget>();

            var perlinTexture = new NoiseTexture(new Color(0.8, 0.8, 0.8), 4.0, NoiseTypes.MARBLE, 0.5, 10.0, 15);

            returnList.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));

            var loader = new ImageReader();

            using (var stream = File.Open("ExternalData/earth.jpg", FileMode.Open))
            {
                var image = loader.Read(stream, StbImage.STBI_rgb);

                var material = new Glossy(new ImageTexture(image.Data, image.Width, image.Height), 0.125);

                returnList.Add(new Sphere(new Vec3(0, 2, 0), 2.0, material));

            }

            return new SceneGenerator(new BvhNode(returnList, 0.0, 1.0), camera);
        }
    }
}
