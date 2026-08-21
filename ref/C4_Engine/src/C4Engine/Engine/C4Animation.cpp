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


#include "C4Animation.h"
#include "C4Models.h"


using namespace C4;


namespace C4
{
	template class Constructable<Animator>;
}


ResourceDescriptor AnimationResource::descriptor("anm", 0, 2049152);


AnimationResource::AnimationResource(const char *name, ResourceCatalog *catalog) : Resource<AnimationResource>(name, catalog)
{
}

AnimationResource::~AnimationResource()
{
}

void AnimationResource::Preprocess(void)
{
	if (GetEndian() != 1)
	{
		AnimationHeader *animationHeader = const_cast<AnimationHeader *>(GetAnimationHeader());
		Reverse(&animationHeader->nodeCount);
		Reverse(&animationHeader->hashTableOffset);
		Reverse(&animationHeader->trackCount);
		
		int32 nodeCount = animationHeader->nodeCount;
		if (animationHeader->hashTableOffset != 0)
		{
			AnimationHeader::HashTable *hashTable = const_cast<AnimationHeader::HashTable *>(animationHeader->GetNodeHashTable());
			Reverse(&hashTable->bucketCount);
			
			AnimationHeader::BucketData *bucketData = hashTable->bucketData;
			AnimationHeader::HashData *hashData = const_cast<AnimationHeader::HashData *>(hashTable->GetHashData());
			
			int32 bucketCount = hashTable->bucketCount;
			for (machine a = 0; a < bucketCount; a++)
			{
				Reverse(&bucketData->bucketSize);
				Reverse(&bucketData->bucketOffset);
				
				unsigned_int32 size = bucketData->bucketSize;
				if (size != 0)
				{
					AnimationHeader::HashData *data = hashData + bucketData->bucketOffset;
					for (unsigned_machine b = 0; b < size; b++)
					{
						Reverse(&data->hashValue);
						Reverse(&data->nodeIndex);
						data++;
					}
				}
				
				bucketData++;
			}
		}
		
		int32 trackCount = animationHeader->trackCount;
		for (machine a = 0; a < trackCount; a++)
		{
			AnimationHeader::TrackData *trackData = &animationHeader->trackData[a];
			Reverse(&trackData->trackType);
			Reverse(&trackData->trackOffset);
			
			void *trackHeader = const_cast<void *>(animationHeader->GetTrackHeader(a));
			switch (trackData->trackType)
			{
				case kTrackTransform:
				{
					TransformTrackHeader *transformTrackHeader = static_cast<TransformTrackHeader *>(trackHeader);
					Reverse(&transformTrackHeader->frameCount);
					Reverse(&transformTrackHeader->frameDuration);
					
					TransformFrameData *frameData = const_cast<TransformFrameData *>(transformTrackHeader->GetFrameData());
					int32 transformCount = nodeCount * transformTrackHeader->frameCount;
					for (machine b = 0; b < transformCount; b++)
					{
						Reverse(&frameData->transform);
						Reverse(&frameData->position);
						frameData++;
					}
					
					break;
				}
				
				case kTrackCue:
				{
					CueTrackHeader *cueTrackHeader = static_cast<CueTrackHeader *>(trackHeader);
					Reverse(&cueTrackHeader->cueCount);
					
					CueData *cueData = cueTrackHeader->cueData;
					int32 cueCount = cueTrackHeader->cueCount;
					for (machine b = 0; b < cueCount; b++)
					{
						Reverse(&cueData->cueType); 
						Reverse(&cueData->cueTime);
						cueData++; 
					} 
					 
					break;
				} 
			}
		}
	}
} 


Animator::Animator(AnimatorType type) : weightInterpolator(1.0F)
{ 
	animatorType = type;
	
	targetModel = nullptr;
	targetNode = nullptr;
	
	animatorNodeStart = 0;
	animatorNodeCount = 0;
	
	storageSize = 0;
	outputStorage = nullptr;
	outputTable = nullptr;
	transformTable = nullptr;
	animatorData = nullptr;
}

Animator::Animator(AnimatorType type, Model *model, Node *node) : weightInterpolator(1.0F)
{
	animatorType = type;
	
	targetModel = model;
	targetNode = (node) ? node : model;
	
	animatorNodeStart = 0;
	animatorNodeCount = 0;
	
	storageSize = 0;
	outputStorage = nullptr;
	outputTable = nullptr;
	transformTable = nullptr;
	animatorData = nullptr;
}

Animator::~Animator()
{
	delete[] outputStorage;
}

Animator *Animator::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kAnimatorFrame:
			
			return (new FrameAnimator);
		
		case kAnimatorMerge:
			
			return (new MergeAnimator);
		
		case kAnimatorBlend:
			
			return (new BlendAnimator);
	}
	
	return (Constructable<Animator>::Construct(data, unpackFlags));
}

Animator *Animator::New(AnimatorType type)
{
	Type	data[2];
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

void Animator::PackType(Packer& data) const
{
	data << animatorType;
}

void Animator::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PackHandle handle = data.BeginChunk('WGHT');
	weightInterpolator.Pack(data, packFlags);
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void Animator::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<Animator>(data, unpackFlags);
}

bool Animator::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'WGHT':
			
			weightInterpolator.Unpack(data, unpackFlags);
			return (true);
	}
	
	return (false);
}

void Animator::AllocateStorage(int32 nodeStart, int32 nodeCount, int32 transformCount, unsigned_int32 dataSize)
{
	animatorNodeStart = nodeStart;
	animatorNodeCount = nodeCount;
	
	unsigned_int32 size = nodeCount * sizeof(AnimatorTransform *) + (transformCount + 1) * sizeof(AnimatorTransform) + dataSize;
	if (storageSize != size)
	{
		storageSize = size;
		delete[] outputStorage;
		outputStorage = new char[size];
	}
	
	outputTable = reinterpret_cast<AnimatorTransform **>(outputStorage);
	AnimatorTransform *transform = reinterpret_cast<AnimatorTransform *>(outputTable + nodeCount);
	transformTable = transform + 1;
	animatorData = transformTable + transformCount;
	
	// Always store an identity transform as the first entry (index -1) in the transform table.
	
	transform->rotation = 1.0F;
	transform->position.Set(0.0F, 0.0F, 0.0F);
}

void Animator::SetTargetModel(Model *model, Node *node)
{
	targetModel = model;
	targetNode = (node) ? node : model;
}

int32 Animator::GetNodeIndex(const Node *target) const
{
	const Model *model = GetTargetModel();
	const Node *node = model->GetFirstSubnode();
	
	int32 index = 0;
	while (node)
	{
		if ((node->GetNodeType() == kNodeModel) || (node->GetNodeFlags() & kNodeAnimateInhibit))
		{
			node = model->GetNextLevelNode(node);
		}
		else
		{
			if (node == target) break;
			
			index++;
			node = model->GetNextNode(node);
		}
	}
	
	index -= animatorNodeStart;
	if ((unsigned_int32) index >= (unsigned_int32) animatorNodeCount) return (-1);
	return (index);
}

void Animator::Update(void)
{
	unsigned_int32 flags = GetCurrentUpdateFlags();
	UpdatableTree<Animator>::Update();
	if (flags & kUpdateOutput) Configure();
}

void Animator::Premove(void)
{
	Animator *animator = GetFirstSubnode();
	while (animator)
	{
		Animator *next = animator->Next();
		animator->Premove();
		animator = next;
	}
	
	weightInterpolator.UpdateValue();
}

void Animator::Preprocess(void)
{
	const Model *model = GetTargetModel();
	const Node *target = GetTargetNode();
	const Node *node = model->GetFirstSubnode();
	
	int32 index = 0;
	if (target != model)
	{
		while (node)
		{
			if ((node->GetNodeType() == kNodeModel) || (node->GetNodeFlags() & kNodeAnimateInhibit))
			{
				node = model->GetNextLevelNode(node);
			}
			else
			{
				if (node == target) break;
				
				index++;
				node = model->GetNextNode(node);
			}
		}
	}
	
	int32 start = index;
	while (node)
	{
		if ((node->GetNodeType() == kNodeModel) || (node->GetNodeFlags() & kNodeAnimateInhibit))
		{
			node = target->GetNextLevelNode(node);
		}
		else
		{
			index++;
			node = target->GetNextNode(node);
		}
	}
	
	animatorNodeStart = start;
	animatorNodeCount = index - start;
	
	Animator *animator = GetFirstSubnode();
	while (animator)
	{
		animator->Preprocess();
		animator = animator->Next();
	}
}

void Animator::Configure(void)
{
}

void Animator::Move(void)
{
	Animator *animator = GetFirstSubnode();
	while (animator)
	{
		Animator *next = animator->Next();
		animator->Move();
		animator = next;
	}
}


MergeAnimator::MergeAnimator() : Animator(kAnimatorMerge)
{
}

MergeAnimator::MergeAnimator(Model *model, Node *node) : Animator(kAnimatorMerge, model, node)
{
}

MergeAnimator::~MergeAnimator()
{
}

void MergeAnimator::Configure(void)
{
	int32 start = GetTargetModel()->GetAnimatedNodeCount();
	int32 finish = 0;
	
	const Animator *animator = GetFirstSubnode();
	while (animator)
	{
		int32 x = animator->GetNodeStart();
		start = Min(start, x);
		finish = Max(finish, x + animator->GetNodeCount());
		
		animator = animator->Next();
	}
	
	int32 s0 = GetNodeStart();
	int32 f0 = s0 + GetNodeCount();
	
	start = Max(start, s0);
	finish = Min(finish, f0);
	
	int32 count = finish - start;
	if (count > 0)
	{
		AllocateStorage(start, count, 0);
		
		AnimatorTransform **outputTable = GetOutputTable();
		for (machine a = 0; a < count; a++) outputTable[a] = nullptr;
		
		animator = GetFirstSubnode();
		while (animator)
		{
			AnimatorTransform *const *table = animator->GetOutputTable();
			
			int32 s = animator->GetNodeStart() - start;
			int32 k = animator->GetNodeCount();
			for (machine a = 0; a < k; a++) outputTable[a + s] = table[a];
			
			animator = animator->Next();
		}
	}
}


BlendAnimator::BlendAnimator() : Animator(kAnimatorBlend)
{
}

BlendAnimator::BlendAnimator(Model *model, Node *node) : Animator(kAnimatorBlend, model, node)
{
}

BlendAnimator::~BlendAnimator()
{
}

void BlendAnimator::Configure(void)
{
	const Animator *animator1 = GetFirstSubnode();
	if (animator1)
	{
		int32 s0 = GetNodeStart();
		int32 f0 = s0 + GetNodeCount();
		
		const Animator *animator2 = animator1->Next();
		if (animator2)
		{
			int32 s1 = Max(animator1->GetNodeStart(), s0);
			int32 f1 = Min(animator1->GetNodeCount() + s1, f0);
			int32 s2 = Max(animator2->GetNodeStart(), s0);
			int32 f2 = Min(animator2->GetNodeCount() + s2, f0);
			
			int32 outerStart = Min(s1, s2);
			int32 outerCount = Max(f1, f2) - outerStart;
			int32 innerStart = Max(s1, s2);
			int32 innerCount = Min(f1, f2) - innerStart;
			
			blendStart = innerStart;
			blendCount = MaxZero(innerCount);
			AllocateStorage(outerStart, outerCount, blendCount);
			
			AnimatorTransform **outputTable = GetOutputTable();
			AnimatorTransform *const *table1 = animator1->GetOutputTable();
			AnimatorTransform *const *table2 = animator2->GetOutputTable();
			
			int32 finish = Min(f1, innerStart);
			for (machine a = s1; a < finish; a++) outputTable[a - outerStart] = table1[a - s1];
			for (machine a = Max(s1, innerStart + innerCount); a < f1; a++) outputTable[a - outerStart] = table1[a - s1];
			
			finish = Min(f2, innerStart);
			for (machine a = s2; a < finish; a++) outputTable[a - outerStart] = table2[a - s2];
			for (machine a = Max(s2, innerStart + innerCount); a < f2; a++) outputTable[a - outerStart] = table2[a - s2];
			
			outputTable += innerStart - outerStart;
			AnimatorTransform *transformTable = GetTransformTable();
			for (machine a = 0; a < innerCount; a++) outputTable[a] = &transformTable[a];
		}
		else
		{
			blendStart = 0;
			blendCount = 0;
			
			int32 x = animator1->GetNodeStart();
			int32 start = Min(x, s0);
			int32 finish = Max(x + animator1->GetNodeCount(), f0);
			int32 count = finish - start;
			AllocateStorage(start, count, 0);
			
			AnimatorTransform **outputTable = GetOutputTable();
			AnimatorTransform *const *table = animator1->GetOutputTable();
			for (machine a = 0; a < count; a++) outputTable[a] = table[a];
		}
	}
}

void BlendAnimator::Move(void)
{
	Animator::Move();
	
	int32 count = blendCount;
	if (count != 0)
	{
		const Animator *animator1 = GetFirstSubnode();
		const Animator *animator2 = animator1->Next();
		
		float w1 = animator1->GetWeightInterpolator()->GetValue();
		float w2 = animator2->GetWeightInterpolator()->GetValue();
		
		if (w1 * w2 != 0.0F)
		{
			float f = 1.0F / (w1 + w2);
			w1 *= f;
			w2 *= f;
			
			int32 s1 = animator1->GetNodeStart();
			int32 s2 = animator2->GetNodeStart();
			
			AnimatorTransform *transformTable = GetTransformTable();
			const AnimatorTransform *const *table1 = animator1->GetOutputTable() + (blendStart - s1);
			const AnimatorTransform *const *table2 = animator2->GetOutputTable() + (blendStart - s2);
			
			for (machine a = 0; a < count; a++)
			{
				const AnimatorTransform *transform1 = table1[a];
				const AnimatorTransform *transform2 = table2[a];
				
				transformTable[a].position = transform1->position * w1 + transform2->position * w2;
				
				const Quaternion& q1 = transform1->rotation;
				const Quaternion& q2 = transform2->rotation;
				Quaternion q3 = (Dot(q1, q2) > 0.0F) ? q1 * w1 + q2 * w2 : q1 * w1 - q2 * w2;
				transformTable[a].rotation = q3.Normalize();
			}
		}
		else if (w1 != 0.0F)
		{
			AnimatorTransform *transformTable = GetTransformTable();
			const AnimatorTransform *const *table1 = animator1->GetOutputTable() + (blendStart - animator1->GetNodeStart());
			
			for (machine a = 0; a < count; a++)
			{
				const AnimatorTransform *transform1 = table1[a];
				transformTable[a].position = transform1->position;
				transformTable[a].rotation = transform1->rotation;
			}
		}
		else
		{
			AnimatorTransform *transformTable = GetTransformTable();
			const AnimatorTransform *const *table2 = animator2->GetOutputTable() + (blendStart - animator2->GetNodeStart());
			
			for (machine a = 0; a < count; a++)
			{
				const AnimatorTransform *transform2 = table2[a];
				transformTable[a].position = transform2->position;
				transformTable[a].rotation = transform2->rotation;
			}
		}
	}
}


FrameAnimator::FrameAnimator() : Animator(kAnimatorFrame)
{
	animationResource = nullptr;
	animationHeader = nullptr;
}

FrameAnimator::FrameAnimator(Model *model, Node *node) : Animator(kAnimatorFrame, model, node)
{
	animationResource = nullptr;
	animationHeader = nullptr;
}

FrameAnimator::~FrameAnimator()
{
	if (animationResource) animationResource->Release();
}

void FrameAnimator::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Animator::Pack(data, packFlags);
	
	if (animationResource)
	{
		PackHandle handle = data.BeginChunk('ANIM');
		data << animationResource->GetName();
		data.EndChunk(handle);
		
		handle = data.BeginChunk('INTP');
		frameInterpolator.Pack(data, packFlags);
		data.EndChunk(handle);
	}
	
	data << TerminatorChunk;
}

void FrameAnimator::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Animator::Unpack(data, unpackFlags);
	UnpackChunkList<FrameAnimator>(data, unpackFlags);
}

bool FrameAnimator::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ANIM':
		{
			ResourceName	name;
			
			data >> name;
			SetAnimation(name);
			return (true);
		}
		
		case 'INTP':
			
			frameInterpolator.Unpack(data, unpackFlags);
			return (true);
	}
	
	return (false);
}

void FrameAnimator::GenerateNodeRemapTable(void)
{
	int16 *remapTable = static_cast<int16 *>(GetAnimatorData());
	
	if (animationHeader->hashTableOffset != 0)
	{
		const AnimationHeader::HashTable *hashTable = animationHeader->GetNodeHashTable();
		const AnimationHeader::HashData *hashData = hashTable->GetHashData();
		unsigned_int32 bucketMask = hashTable->bucketCount - 1;
		
		const Node *target = GetTargetNode();
		const Node *node = (target == GetTargetModel()) ? target->GetFirstSubnode() : target;
		
		while (node)
		{
			if ((node->GetNodeType() == kNodeModel) || (node->GetNodeFlags() & kNodeAnimateInhibit))
			{
				node = target->GetNextLevelNode(node);
			}
			else
			{
				unsigned_int32 hash = node->GetNodeHash();
				const AnimationHeader::BucketData *bucketData = &hashTable->bucketData[hash & bucketMask];
				
				int32 index = -1;
				int32 size = bucketData->bucketSize;
				const AnimationHeader::HashData *data = hashData + bucketData->bucketOffset;
				for (machine a = 0; a < size; a++)
				{
					if (data->hashValue == hash)
					{
						index = data->nodeIndex;
						break;
					}
					
					data++;
				}
				
				*remapTable++ = (int16) index;
				
				node = target->GetNextNode(node);
			}
		}
	}
	else
	{
		int32 count = GetNodeCount();
		for (machine a = 0; a < count; a++) remapTable[a] = (int16) a;
	}
}

void FrameAnimator::ExecuteAnimationFrame(float frame)
{
	float	f1, f2;
	
	PositiveFloorCeil(frame, &f1, &f2);
	float t2 = frame - f1;
	float t1 = 1.0F - t2;
	
	const TransformTrackHeader *track = transformTrackHeader;
	if (track)
	{
		AnimatorTransform *transformTable = GetTransformTable();
		const int16 *remapTable = static_cast<int16 *>(GetAnimatorData());
		
		int32 animationCount = animationHeader->nodeCount;
		const TransformFrameData *data = track->GetFrameData();
		const TransformFrameData *frameData1 = data + (int32) f1 * animationCount;
		const TransformFrameData *frameData2 = data + (int32) f2 * animationCount;
		
		int32 count = GetNodeCount();
		for (machine a = 0; a < count; a++)
		{
			Quaternion		q1, q2;
			
			int32 index = remapTable[a];
			const TransformFrameData *fd1 = frameData1 + index;
			const TransformFrameData *fd2 = frameData2 + index;
			
			transformTable[a].position = fd1->position * t1 + fd2->position * t2;
			
			q1.SetRotationMatrix(fd1->transform);
			q2.SetRotationMatrix(fd2->transform);
			Quaternion q3 = (Dot(q1, q2) > 0.0F) ? q1 * t1 + q2 * t2 : q1 * t1 - q2 * t2;
			transformTable[a].rotation = q3.Normalize();
		}
	}
	
	animationFrame = frame;
}

void FrameAnimator::Configure(void)
{
	int32 count = GetNodeCount();
	AllocateStorage(GetNodeStart(), count, count, count * 2);
	
	AnimatorTransform **outputTable = GetOutputTable();
	AnimatorTransform *transformTable = GetTransformTable();
	for (machine a = 0; a < count; a++) outputTable[a] = &transformTable[a];
	
	if (animationHeader) GenerateNodeRemapTable();
}

void FrameAnimator::Move(void)
{
	Animator::Move();
	
	const AnimationHeader *header = animationHeader;
	if (header)
	{
		SubrangeData	subrangeData;
		
		float value = frameInterpolator.UpdateValue(&subrangeData);
		
		if (animationHeader != header)
		{
			float frame = frameInterpolator.GetValue() * frameFrequency;
			if ((GetWeightInterpolator()->GetValue() > 0.0F) || (animationFrame < 0.0F)) ExecuteAnimationFrame(frame);
		}
		else
		{
			float frame = value * frameFrequency;
			if (animationFrame != frame)
			{
				if ((GetWeightInterpolator()->GetValue() > 0.0F) || (animationFrame < 0.0F))
				{
					ExecuteAnimationFrame(frame);
					
					const CueTrackHeader *track = cueTrackHeader;
					if (track)
					{
						int32 cueCount = track->cueCount;
						int32 subrangeCount = subrangeData.subrangeCount;
						const Range<float> *subrange = subrangeData.subrange;
						
						for (machine a = 0; a < subrangeCount; a++)
						{
							float begin = subrange->min;
							float end = subrange->max;
							
							if (begin < end)
							{
								const CueData *cueData = track->cueData;
								for (machine b = 0; b < cueCount; b++)
								{
									float time = cueData->cueTime;
									if (time > end) break;
									
									if (time > begin) PostEvent(cueData->cueType);
									cueData++;
								}
							}
							else
							{
								const CueData *cueData = track->cueData + (cueCount - 1);
								for (machine b = 0; b < cueCount; b++)
								{
									float time = cueData->cueTime;
									if (time < end) break;
									
									if (time < begin) PostEvent(cueData->cueType);
									cueData--;
								}
							}
							
							subrange++;
						}
					}
				}
			}
		}
	}
	else
	{
		AnimatorTransform *transformTable = GetTransformTable();
		
		int32 count = GetNodeCount();
		for (machine a = 0; a < count; a++)
		{
			transformTable[a].rotation = 1.0F;
			transformTable[a].position.Set(0.0F, 0.0F, 0.0F);
		}
	}
}

void FrameAnimator::SetAnimationHeader(const AnimationHeader *header)
{
	animationHeader = header;
	transformTrackHeader = nullptr;
	cueTrackHeader = nullptr;
	
	int32 trackCount = header->trackCount;
	for (machine a = 0; a < trackCount; a++)
	{
		TrackType type = header->trackData[a].trackType;
		if (type == kTrackTransform)
		{
			transformTrackHeader = static_cast<const TransformTrackHeader *>(header->GetTrackHeader(a));
		}
		else if (type == kTrackCue)
		{
			cueTrackHeader = static_cast<const CueTrackHeader *>(header->GetTrackHeader(a));
		}
	}
	
	if (transformTrackHeader)
	{
		float frameCount = (float) (transformTrackHeader->frameCount - 1);
		float frameDuration = transformTrackHeader->frameDuration;
		
		animationDuration = frameDuration * frameCount;
		frameFrequency = 1.0F / frameDuration;
		animationFrame = -1.0F;
		
		frameInterpolator.Set(0.0F, 1.0F, kInterpolatorStop);
		frameInterpolator.SetRange(0.0F, animationDuration);
	}
	
	if (GetAnimatorData()) GenerateNodeRemapTable();
}

void FrameAnimator::SetAnimation(const char *name)
{
	if (animationResource)
	{
		animationResource->Release();
		animationResource = nullptr;
		animationHeader = nullptr;
	}
	
	if (name)
	{
		AnimationResource *resource = AnimationResource::Get(name);
		if (resource)
		{
			animationResource = resource;
			SetAnimationHeader(resource->GetAnimationHeader());
		}
	}
}

// ZYURVUR
