﻿using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Paths;
using BEPUphysics.Paths.PathFollowing;
using BEPUutilities;
using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEPUbenchmark.Benchmarks
{
	public class PathFollowingBenchmark : Benchmark
	{
		private EntityMover mover;
		private Path<Quaternion> orientationPath;
		private Path<Vector3> positionPath;
		private EntityRotator rotator;
		private Fix64 pathTime;

		protected override void InitializeSpace()
		{
			Entity movingEntity;
			//The moving entity can be either kinematic or dynamic; the EntityMover/Rotator works for either.
			movingEntity = new Box(new Vector3(-10, 0, -10), 3, 1, 1);

			//We're going to use a speed-controlled curve that wraps another curve.
			//This is the internal curve.
			//Speed-controlled curves let the user specify speeds at which an evaluator
			//will move along the curve.  Non-speed controlled curves can move at variable
			//rates based on the interpolation.
			var wrappedPositionCurve = new CardinalSpline3D();

			//Since the endpoints don't overlap, just reverse direction when they're hit.
			//The default is wrapping around; if there's a distance between the starting
			//and ending endpoints, the entity will jump very quickly (smashing anything in the way).
			wrappedPositionCurve.PreLoop = CurveEndpointBehavior.Mirror;
			wrappedPositionCurve.PostLoop = CurveEndpointBehavior.Mirror;

			//Start the curve up above the blocks.
			//There's two control points because the very first and very last control points
			//aren't actually reached by the curve in a CardinalSpline3D; they are used
			//to define the tangents on the interior points.
			wrappedPositionCurve.ControlPoints.Add(-1, new Vector3(0, 30, 0));
			wrappedPositionCurve.ControlPoints.Add(0, new Vector3(0, 20, 0));
			//Add a bunch of random control points to the curve.
			var random = new Random(0);
			for (int i = 1; i <= 10; i++)
			{
				wrappedPositionCurve.ControlPoints.Add(i, new Vector3(
															  (Fix64)random.NextDouble() * 20 - 10,
															  (Fix64)random.NextDouble() * 12,
															  (Fix64)random.NextDouble() * 20 - 10));
			}

			positionPath = wrappedPositionCurve;

			//There's also a constant speed and variable speed curve type that can be used.
			//Try the following instead to move the entity at a constant rate:
			//positionPath = new ConstantLinearSpeedCurve(5, wrappedPositionCurve);

			var slerpCurve = new QuaternionSlerpCurve();
			slerpCurve.ControlPoints.Add(0, Quaternion.Identity);
			slerpCurve.ControlPoints.Add(1, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver2));
			slerpCurve.ControlPoints.Add(2, Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.Pi));
			slerpCurve.ControlPoints.Add(3, Quaternion.CreateFromAxisAngle(Vector3.Up, 3 * MathHelper.PiOver2));
			slerpCurve.ControlPoints.Add(4, Quaternion.Identity);

			slerpCurve.PostLoop = CurveEndpointBehavior.Mirror;
			orientationPath = slerpCurve;


			mover = new EntityMover(movingEntity);
			//Offset the place that the mover tries to reach a little.
			//Now, when the entity spins, it acts more like a hammer swing than a saw.
			mover.LocalOffset = new Vector3(3, 0, 0);
			rotator = new EntityRotator(movingEntity);

			//Add the entity and movers to the space.
			Space.Add(movingEntity);
			Space.Add(mover);
			Space.Add(rotator);

			//Add some extra stuff to the space.
			Space.Add(new Box(new Vector3(0, -5, 0), 25, 10, 25));

			int numColumns = 7;
			int numRows = 7;
			int numHigh = 3;
			Fix64 xSpacing = 2.09m;
			Fix64 ySpacing = 2.08m;
			Fix64 zSpacing = 2.09m;
			for (int i = 0; i < numRows; i++)
				for (int j = 0; j < numColumns; j++)
					for (int k = 0; k < numHigh; k++)
					{
						Space.Add(new Box(new Vector3(
											  xSpacing * i - (numRows - 1) * xSpacing / 2,
											  1.58m + k * (ySpacing),
											  2 + zSpacing * j - (numColumns - 1) * zSpacing / 2),
										  2, 2, 2, 10));
					}
		}

		protected override void Step()
		{
			pathTime += Space.TimeStepSettings.TimeStepDuration;
			mover.TargetPosition = positionPath.Evaluate(pathTime);
			rotator.TargetOrientation = orientationPath.Evaluate(pathTime);
			base.Step();
		}
	}
}
