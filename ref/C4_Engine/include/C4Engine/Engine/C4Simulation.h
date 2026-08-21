//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4Simulation_h
#define C4Simulation_h


namespace C4
{
	enum
	{
		kSlowMotionMultiplier			= 1,
		kPhysicsTimeStep				= 16 * kSlowMotionMultiplier,
		kMaxPhysicsStepCount			= 2,
		kMaxConstraintIterationCount	= 100,
		kRigidBodySleepStepCount		= 20
	};
	
	
	const float kTimeStep = (float) (kPhysicsTimeStep / kSlowMotionMultiplier) * 0.001F;
	const float kInverseTimeStep = 1.0F / kTimeStep;
	const float kInversePhysicsTimeStep = 1.0F / (float) kPhysicsTimeStep;
	const float kContactStabilizeFactor = kInverseTimeStep * 0.25F;
	
	
	const float kMinRigidBodyMass = 1.0e-3F;
	const float kDefaultMaxLinearSpeed = 200.0F;
	const float kDefaultMaxAngularSpeed = 25.132741F;	// 8 pi
	
	
	const float kRigidBodySleepBoxSize = 1.0e-3F;
	const float kCollisionSweepEpsilon = 1.0e-3F;
	const float kContactEpsilon = -0.005F;
	
	
	const float kMaxShapeShrinkSize = 0.03125F;
	const float kSupportPointTolerance = 1.0e-5F;
	const float kSimplexVertexEpsilon = 1.0e-5F;
	const float kSimplexDimensionEpsilon = 1.0e-3F;
	const float kIntersectionDisplacementEpsilon = 1.0e-4F;
	const float kSemiInfiniteIntersectionDepth = -64.0F;
	
	
	const float kMaxSubcontactSquaredDelta = 1.0e-4F;
	const float kMaxSubcontactSquaredTangentialSeparation = 0.001F;
};


#endif

// ZYURVUR
