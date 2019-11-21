using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathUtilities;

namespace RenderDataStructures.Shapes
{
    public class BvhNode : IHitTarget
    {
        private IHitTarget Left { get; }
        private IHitTarget Right { get; }
        private BoundingBox BBox { get; }

        public BvhNode(List<IHitTarget> p_list, double p_time1, double p_time2)
        {
            var rng = new Random();

            var axisPicker = (int)(3 * rng.NextDouble());

            switch (axisPicker)
            {
                case 0:
                    p_list.Sort(new BoundingBoxXComparer());
                    break;
                case 1:
                    p_list.Sort(new BoundingBoxYComparer());
                    break;
                default:
                    p_list.Sort(new BoundingBoxZComparer());
                    break;
            }

            switch (p_list.Count)
            {
                case 1:
                    Left = Right = p_list.First();
                    break;
                case 2:
                    Left = p_list.First();
                    Right = p_list.Last();
                    break;
                default:
                {
                    var listCopy = new Queue<IHitTarget>(p_list);

                    var leftList = new List<IHitTarget>();
                    var rightList = new List<IHitTarget>();

                    for (var i = 0; i < listCopy.Count / 2; ++i)
                    {
                        leftList.Add(listCopy.Dequeue());
                    }

                    while (listCopy.Any())
                    {
                        rightList.Add(listCopy.Dequeue());
                    }

                    Left = new BvhNode(leftList, p_time1, p_time2);
                    Right = new BvhNode(rightList, p_time1, p_time2);
                    break;
                }
            }

            var leftBox = new BoundingBox(new Vec3(0), new Vec3(0));
            var rightBox = new BoundingBox(new Vec3(0), new Vec3(0));

            if (!Left.GenerateBoundingBox(p_time1, p_time2, ref leftBox) ||
                 !Right.GenerateBoundingBox(p_time1, p_time2, ref rightBox))
            {
                throw new Exception("No bounding box in BvhNode constructor...");
            }

            BBox = BoundingBox.GetSurroundingBox(leftBox, rightBox);
        }

        public bool WasHit(Ray p_ray, double p_tMin, double p_tMax, ref HitRecord p_hitRecord)
        {
            if (BBox.WasHit(p_ray, p_tMin, p_tMax))
            {
                var leftHitRecord = new HitRecord();
                var rightHitRecord = new HitRecord();
                var hitLeft = Left.WasHit(p_ray, p_tMin, p_tMax, ref leftHitRecord);
                var hitRight = Right.WasHit(p_ray, p_tMin, p_tMax, ref rightHitRecord);
                if (hitLeft && hitRight)
                {
                    p_hitRecord = leftHitRecord.T < rightHitRecord.T ? leftHitRecord : rightHitRecord;

                    return true;
                }

                if (hitLeft)
                {
                    p_hitRecord = leftHitRecord;
                    return true;
                }

                if (hitRight)
                {
                    p_hitRecord = rightHitRecord;
                    return true;
                }

                return false;
            }

            return false;
        }

        public bool GenerateBoundingBox(double p_time1, double p_time2, ref BoundingBox p_box)
        {
            p_box = BBox;
            return true;
        }
    }

    internal class BoundingBoxXComparer : IComparer<IHitTarget>
    {
        public int Compare(IHitTarget p_x, IHitTarget p_y)
        {
            var leftBox = new BoundingBox(new Vec3(0), new Vec3(0));
            var rightBox = new BoundingBox(new Vec3(0), new Vec3(0));
            if (!p_x.GenerateBoundingBox(0, 0, ref leftBox) || !p_y.GenerateBoundingBox(0, 0, ref rightBox))
            {
                throw new Exception("No bounding box in BvhNode constructor...");
            }
            if (leftBox.Min.X - rightBox.Min.X < 0.0)
            {
                return -1;
            }

            return 1;
        }
    }

    internal class BoundingBoxYComparer : IComparer<IHitTarget>
    {
        public int Compare(IHitTarget p_x, IHitTarget p_y)
        {
            var leftBox = new BoundingBox(new Vec3(0), new Vec3(0));
            var rightBox = new BoundingBox(new Vec3(0), new Vec3(0));
            if (!p_x.GenerateBoundingBox(0, 0, ref leftBox) || !p_y.GenerateBoundingBox(0, 0, ref rightBox))
            {
                throw new Exception("No bounding box in BvhNode constructor...");
            }
            if (leftBox.Min.Y - rightBox.Min.Y < 0.0)
            {
                return -1;
            }

            return 1;
        }
    }

    internal class BoundingBoxZComparer : IComparer<IHitTarget>
    {
        public int Compare(IHitTarget p_x, IHitTarget p_y)
        {
            var leftBox = new BoundingBox(new Vec3(0), new Vec3(0));
            var rightBox = new BoundingBox(new Vec3(0), new Vec3(0));
            if (!p_x.GenerateBoundingBox(0, 0, ref leftBox) || !p_y.GenerateBoundingBox(0, 0, ref rightBox))
            {
                throw new Exception("No bounding box in BvhNode constructor...");
            }
            if (leftBox.Min.Z - rightBox.Min.Z < 0.0)
            {
                return -1;
            }

            return 1;
        }
    }
}
