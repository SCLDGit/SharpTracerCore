using System;
using System.Collections.Generic;
using MathUtilities;
using RenderDataStructures.Cameras;
using RenderDataStructures.Materials;
using RenderDataStructures.Shapes;
using Math = System.Math;

namespace RenderHandler
{
    internal class SceneGenerator
    {
        public SceneGenerator(List<IHitTarget> p_world, ICamera p_camera)
        {
            World = p_world;
            Camera = p_camera;
        }

        public ICamera Camera { get; set; }
        public List<IHitTarget> World { get; set; }

        internal static SceneGenerator GenerateThreeSphereScene(RenderParameters p_renderParameters)
        {
            var lookFrom = new Vec3(0, 0, 0);
            var lookAt = new Vec3(0, 0, -1);
            const double focalDistance = 2;
            const double aperture = 0.0;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 90, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance);


            var worldList = new List<IHitTarget>();

            worldList.Add(new Sphere(new Vec3(0, 0, -1), 0.5, new Lambertian(new Color(0.8, 0.3, 0.3))));
            worldList.Add(new Sphere(new Vec3(0, -100.5, -1), 100, new Lambertian(new Color(0.8, 0.8, 0))));
            worldList.Add(new Sphere(new Vec3(1, 0, -1), 0.5, new Glossy(new Color(0.8, 0.6, 0.2), 1.0)));
            worldList.Add(new Sphere(new Vec3(-1, 0, -1), 0.5, new Dielectric(new Color(1.0, 1.0, 1.0), 1.5)));
            worldList.Add(new Sphere(new Vec3(-1, 0, -1), -0.45, new Dielectric(new Color(1.0, 1.0, 1.0), 1.5)));

            var newScene = new SceneGenerator(worldList, camera);

            return newScene;
        }

        internal static SceneGenerator GenerateRandomScene(RenderParameters p_renderParameters, int p_numberOfObjects)
        {
            var lookFrom = new Vec3(-13, 2, -3);
            var lookAt = new Vec3(0, 0, 0);
            const double focalDistance = 10.0;
            const double aperture = 0.001;
            var camera = new ThinLensCamera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, p_renderParameters.XResolution / (float)p_renderParameters.YResolution, aperture, focalDistance);


            var rng = new Random();

            var worldList = new List<IHitTarget>
            {
                new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(new Color(0.5, 0.5, 0.5)))
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
                        worldList.Add(new Sphere(center, 0.2, new Lambertian(new Color(rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble(), rng.NextDouble() * rng.NextDouble())) ));
                    }
                    else if ( materialPicker < 0.8 )
                    {
                        worldList.Add(new Sphere(center, 0.2, new Glossy(new Color(0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble()), 0.5 * (1 + rng.NextDouble())), 0.5 * rng.NextDouble())));
                    }
                    else
                    {
                        worldList.Add(new Sphere(center, 0.2, new Dielectric(new Color(1, 1, 1), 1.5) ));
                    }
                }
            }

            worldList.Add(new Sphere(new Vec3(0, 1, 0), 1.0, new Dielectric(new Color(1, 1, 1), 1.5) ));
            worldList.Add(new Sphere(new Vec3(-4, 1, 0), 1.0, new Lambertian(new Color(0.4, 0.2, 0.1)) ));
            worldList.Add(new Sphere(new Vec3(4, 1, 0), 1.0, new Glossy(new Color(0.7, 0.6, 0.5), 0.05)) );

            var newScene = new SceneGenerator(worldList, camera);

            return newScene;
        }
    }
}
