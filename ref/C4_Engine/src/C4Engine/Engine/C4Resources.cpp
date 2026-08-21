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


#include "C4Engine.h"


using namespace C4;


ResourceMgr *C4::TheResourceMgr = nullptr;


namespace C4
{
	template <> ResourceMgr Manager<ResourceMgr>::managerObject(0);
	template <> ResourceMgr **Manager<ResourceMgr>::managerPointer = &TheResourceMgr;
}


PackFile::PackFile(const char *fileName)
{
	locationPath = fileName;
	locationPath[Text::GetResourceNameLength(locationPath)] = 0;
	
	mapStorage = nullptr;
}

PackFile::~PackFile()
{
	delete[] mapStorage;
}

void PackFile::ReverseFileEntry(PackFileEntry *entry, const void *base)
{
	Reverse(&entry->leftOffset);
	Reverse(&entry->rightOffset);
	Reverse(&entry->dataStart);
	Reverse(&entry->dataSize);
	
	const PackFileEntry *leftEntry = entry->GetLeftEntry(base);
	if (leftEntry) ReverseFileEntry(const_cast<PackFileEntry *>(leftEntry), base);
	
	const PackFileEntry *rightEntry = entry->GetRightEntry(base);
	if (rightEntry) ReverseFileEntry(const_cast<PackFileEntry *>(rightEntry), base);
}

void PackFile::ReverseTypeEntry(PackTypeEntry *entry, const void *base)
{
	Reverse(&entry->leftOffset);
	Reverse(&entry->rightOffset);
	Reverse(&entry->resourceType);
	Reverse(&entry->fileRootOffset);
	
	const PackTypeEntry *leftEntry = entry->GetLeftEntry(base);
	if (leftEntry) ReverseTypeEntry(const_cast<PackTypeEntry *>(leftEntry), base);
	
	const PackTypeEntry *rightEntry = entry->GetRightEntry(base);
	if (rightEntry) ReverseTypeEntry(const_cast<PackTypeEntry *>(rightEntry), base);
	
	const PackFileEntry *typeEntry = entry->GetFileRootEntry(base);
	if (typeEntry) ReverseFileEntry(const_cast<PackFileEntry *>(typeEntry), base);
}

void PackFile::ReverseDirectoryEntry(PackDirectoryEntry *entry, const void *base)
{
	Reverse(&entry->leftOffset);
	Reverse(&entry->rightOffset);
	Reverse(&entry->typeRootOffset);
	Reverse(&entry->directoryRootOffset);
	
	const PackDirectoryEntry *leftEntry = entry->GetLeftEntry(base);
	if (leftEntry) ReverseDirectoryEntry(const_cast<PackDirectoryEntry *>(leftEntry), base);
	
	const PackDirectoryEntry *rightEntry = entry->GetRightEntry(base);
	if (rightEntry) ReverseDirectoryEntry(const_cast<PackDirectoryEntry *>(rightEntry), base);
	
	const PackTypeEntry *typeEntry = entry->GetTypeRootEntry(base);
	if (typeEntry) ReverseTypeEntry(const_cast<PackTypeEntry *>(typeEntry), base);
	
	const PackDirectoryEntry *directoryEntry = entry->GetDirectoryRootEntry(base);
	if (directoryEntry) ReverseDirectoryEntry(const_cast<PackDirectoryEntry *>(directoryEntry), base);
}

ResourceResult PackFile::Open(const char *name)
{
	PackHeader	packHeader;
	
	if (packFile.Open(name) != kFileOkay) return (kResourceInvalidPackFile);
	
	if (packFile.Read(&packHeader, sizeof(PackHeader)) != kFileOkay)
	{
		packFile.Close();
		return (kResourceInvalidPackFile);
	}
	
	if (packHeader.endian != 1)
	{
		Reverse(&packHeader.version);
		Reverse(&packHeader.packFileType);
		Reverse(&packHeader.mapSize);
	}
	
	if (packHeader.packFileType != kPackFileDefault)
	{
		packFile.Close(); 
		return (kResourceInvalidPackFile);
	} 
	 
	unsigned_int32 size = packHeader.mapSize; 
	char *storage = new char[size];
	 
	if (packFile.Read(storage, size) != kFileOkay)
	{
		delete[] storage;
		packFile.Close(); 
		return (kResourceInvalidPackFile);
	}
	
	mapStorage = storage; 
	dataStart = size + sizeof(PackHeader);
	
	if (packHeader.endian != 1) ReverseDirectoryEntry(const_cast<PackDirectoryEntry *>(GetRootDirectory()), storage);
	
	return (kResourceOkay);
}

int32 PackFile::CompareFileNames(const char *name, const char *root)
{
	for (machine a = 0;; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(name + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(root + a);
		
		if (x == 0) return (-(y == 0));
		if (y == 0) return (1);
		
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		
		if (x != y) return (x > y);
	}
}

int32 PackFile::CompareDirectoryNames(const char *name, const char *root, int32 max)
{
	for (machine a = 0; a < max; a++)
	{
		unsigned_int32 x = *reinterpret_cast<const unsigned_int8 *>(name + a);
		unsigned_int32 y = *reinterpret_cast<const unsigned_int8 *>(root + a);
		
		if (x == 0) return (0);
		if (y == 0) return (1);
		
		if (x - 'a' < 26UL) x -= 32;
		if (y - 'a' < 26UL) y -= 32;
		
		if (x != y) return (x > y);
	}
	
	return (-(root[max] == 0));
}

const PackFileEntry *PackFile::FindFileEntry(const PackTypeEntry *type, const void *base, const char *name)
{
	const PackFileEntry *entry = type->GetFileRootEntry(base);
	while (entry)
	{
		int32 a = CompareFileNames(name, entry->name);
		if (a == 0) entry = entry->GetLeftEntry(base);
		else if (a > 0) entry = entry->GetRightEntry(base);
		else break;
	}
	
	return (entry);
}

const PackTypeEntry *PackFile::FindTypeEntry(const PackDirectoryEntry *directory, const void *base, ResourceType type)
{
	const PackTypeEntry *entry = directory->GetTypeRootEntry(base);
	while (entry)
	{
		ResourceType t = entry->resourceType;
		if (type < t) entry = entry->GetLeftEntry(base);
		else if (type > t) entry = entry->GetRightEntry(base);
		else break;
	}
	
	return (entry);
}

const PackDirectoryEntry *PackFile::FindDirectoryEntry(const PackDirectoryEntry *directory, const void *base, const char *name, int32 len)
{
	const PackDirectoryEntry *entry = directory->GetDirectoryRootEntry(base);
	while (entry)
	{
		int32 a = CompareDirectoryNames(name, entry->name, len);
		if (a == 0) entry = entry->GetLeftEntry(base);
		else if (a > 0) entry = entry->GetRightEntry(base);
		else break;
	}
	
	return (entry);
}

const PackFileEntry *PackFile::FindResource(ResourceType type, const char *name) const
{
	const PackFileEntry *fileEntry = nullptr;
	
	const PackDirectoryEntry *directoryEntry = GetRootDirectory();
	do
	{
		int32 len = Text::FindChar(name, '/');
		if (len > 0)
		{
			directoryEntry = FindDirectoryEntry(directoryEntry, mapStorage, name, len);
			name += len + 1;
		}
		else
		{
			const PackTypeEntry *typeEntry = FindTypeEntry(directoryEntry, mapStorage, type);
			if (typeEntry) fileEntry = FindFileEntry(typeEntry, mapStorage, name);
			break;
		}
	} while (directoryEntry);
	
	return (fileEntry);
}


ResourceDescriptor::ResourceDescriptor(const char *type, unsigned_int32 flags, unsigned_int32 cacheSize, const char *defaultName)
{
	resourceType = (type[0] << 16) | (type[1] << 8) | type[2];
	
	resourceExtension[0] = '.';
	resourceExtension[1] = type[0];
	resourceExtension[2] = type[1];
	resourceExtension[3] = type[2];
	resourceExtension[4] = 0;
	
	resourceFlags = flags;
	resourceCacheSize = cacheSize;
	
	defaultResourceName = defaultName;
}


ResourceBase::ResourceBase(const char *name, ResourceCatalog *catalog)
{
	resourceName = name;
	resourceSize = 0;
	resourceData = nullptr;
	
	resourceCatalog = catalog;
	resourceTracker = nullptr;
}

ResourceBase::~ResourceBase()
{
	delete[] resourceData;
}

int32 ResourceBase::Release(void)
{
	if (resourceTracker) return (resourceTracker->ReleaseResource(this));
	return (Shared::Release());
}

unsigned_int32 ResourceBase::Hash(KeyType key)
{

	unsigned_int32 hash = 0x518D42C5;
		
	const char *string = key;
	for (;;)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(string);
		if (c == 0) break;
		
		hash = hash * c + (c ^ 0x7F) + 1;
		string++;
	}
	
	return (hash);
}

ResourceResult ResourceBase::OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, ResourceLocation *location)
{
	ResourceResult result = resourceCatalog->OpenLoader(loader, descriptor, flags, resourceName, location);
	if ((result != kResourceOkay) && (!(flags & kResourceNoDefault)))
	{
		const char *defaultName = descriptor->GetDefaultResourceName();
		if (defaultName[0] != 0)
		{
			result = resourceCatalog->OpenLoader(loader, descriptor, flags, defaultName, location);
		}
	}
	
	loader->memorySize += ((descriptor->GetFlags() & kResourceExtraByte) != 0);
	return (result);
}

ResourceResult ResourceBase::Load(ResourceLoader *loader)
{
	char *data = new char[loader->GetMemorySize()];
	
	unsigned_int32 size = loader->GetDataSize();
	ResourceResult result = loader->Read(data, 0, size);
	if (result != kResourceOkay)
	{
		delete[] data;
		return (result);
	}
	
	resourceSize = size;
	resourceData = data;
	
	Preprocess();
	return (kResourceOkay);
}

void ResourceBase::Preprocess(void)
{
}


ResourceTracker::ResourceTracker(const ResourceDescriptor *descriptor) : resourceHashTable(16, 4)
{
	resourceType = descriptor->GetType();
	
	maxCacheSize = descriptor->GetCacheSize();
	currentCacheSize = 0;
}

ResourceTracker::~ResourceTracker()
{
}

void ResourceTracker::AddResource(ResourceBase *resource)
{
	resource->Retain();
	resourceHashTable.Insert(resource);
	resource->resourceTracker = this;
}

int32 ResourceTracker::RetainResource(ResourceBase *resource)
{
	int32 count = resource->Retain();
	if (count == 2)
	{
		currentCacheSize = MaxZero(currentCacheSize - resource->GetSize());
		cacheList.Remove(resource);
	}
	
	return (count);
}

int32 ResourceTracker::ReleaseResource(ResourceBase *resource)
{
	int32 count = resource->Shared::Release();
	if (count == 1)
	{
		unsigned_int32 size = resource->GetSize();
		if ((size > maxCacheSize / 4) || (!resource->GetData()))
		{
			resource->Shared::Release();
			return (0);
		}
		
		unsigned_int32 cacheSize = currentCacheSize + size;
		while (cacheSize > maxCacheSize)
		{
			ResourceBase *cachedResource = cacheList.First();
			if (!cachedResource)
			{
				cacheSize = 0;
				break;
			}
			
			cacheSize -= cachedResource->GetSize();
			cachedResource->Shared::Release();
		}
		
		currentCacheSize = cacheSize;
		cacheList.Append(resource);
	}
	
	return (count);
}


ResourceLoader::ResourceLoader()
{
}

ResourceLoader::~ResourceLoader()
{
}

ResourceResult ResourceLoader::Open(const char *filename)
{
	if (loaderFile.Open(filename) != kFileOkay) return (kResourceNotFound);
	
	dataStart = 0;
	dataSize = loaderFile.GetSize();
	memorySize = dataSize;
	
	resourceFile = &loaderFile;
	return (kResourceOkay);
}

void ResourceLoader::Open(File *file, unsigned_int32 start, unsigned_int32 size)
{
	resourceFile = file;
	
	dataStart = start;
	dataSize = size;
	memorySize = size;
}

void ResourceLoader::Close()
{
	loaderFile.Close();
}

ResourceResult ResourceLoader::Read(void *buffer, unsigned_int32 start, unsigned_int32 size)
{
	resourceFile->SetPosition(dataStart + start);
	if (resourceFile->Read(buffer, size) != kFileOkay) return (kResourceLoadFailed);
	return (kResourceOkay);
}


ResourceCatalog::ResourceCatalog(const char *root)
{
	rootPath = root;
	rootPathLength = Text::GetTextLength(root);
}

ResourceCatalog::~ResourceCatalog()
{
}

ResourceTracker *ResourceCatalog::GetTracker(const ResourceDescriptor *descriptor)
{
	ResourceType type = descriptor->GetType();
	ResourceTracker *tracker = trackerMap.Find(type);
	if (!tracker)
	{
		tracker = new ResourceTracker(descriptor);
		trackerMap.Insert(tracker);
	}
	
	return (tracker);
}

bool ResourceCatalog::ResourceFilter(const char *name, unsigned_int32 flags, void *cookie)
{
	if (flags & kFileDirectory) return (name[0] != '.');
	
	int32 len = Text::GetTextLength(name);
	if ((len > 4) && (name[len - 4] == '.'))
	{
		unsigned_int8 x = name[len - 3];
		unsigned_int8 y = name[len - 2];
		unsigned_int8 z = name[len - 1];
		
		if (x - 65U < 26U) x += 32;
		if (y - 65U < 26U) y += 32;
		if (z - 65U < 26U) z += 32;
		
		ResourceType type = *static_cast<ResourceType *>(cookie);
		if ((x == (unsigned_int8) (type >> 16)) && (y == (unsigned_int8) ((type >> 8) & 0xFF)) && (z == (unsigned_int8) (type & 0xFF))) return (true);
	}
	
	return (false);
}

void ResourceCatalog::BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const
{
}


GenericResourceCatalog::GenericResourceCatalog(const char *root) : ResourceCatalog(root)
{
}

GenericResourceCatalog::~GenericResourceCatalog()
{
}

ResourceResult GenericResourceCatalog::OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, const char *name, ResourceLocation *location)
{
	ResourcePath	path;
	
	GetResourcePath(descriptor, name, &path);
	return (loader->Open(path));
}

void GenericResourceCatalog::CloseLoader(ResourceLoader *loader)
{
	loader->Close();
}

void GenericResourceCatalog::GetResourcePath(const ResourceDescriptor *descriptor, const char *name, ResourcePath *path) const
{
	*path = GetRootPath();
	
	char *c = *path;
	unsigned_int32 len = GetRootPathLength();
	len += Text::CopyText(name, &c[len], kMaxResourcePathLength - 4 - len);
	
	if (!(descriptor->GetFlags() & kResourceDontAppendType))
	{
		ResourceType type = descriptor->GetType();
		c[len++] = '.';
		c[len++] = (char) (type >> 16);
		c[len++] = (char) ((type >> 8) & 0xFF);
		c[len++] = (char) (type & 0xFF);
		c[len] = 0;
	}
}

void GenericResourceCatalog::GetResourcePath(const ResourceDescriptor *descriptor, const char *name, const ResourceLocation *location, ResourcePath *path) const
{
	ResourcePath fullName(location->GetPath());
	if (fullName[0] != 0) fullName += '/';
	fullName += name;
	
	GetResourcePath(descriptor, fullName, path);
}

void GenericResourceCatalog::BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const
{
	ResourcePath path(GetRootPath());
	if (subdir) path += subdir;
	
	ResourceType type = descriptor->GetType();
	FileMgr::BuildFileList(path, list, &ResourceFilter, &type);
}


VirtualResourceCatalog::VirtualResourceCatalog(const char *root) : ResourceCatalog(root)
{
	List<FileReference>		fileList;
	
	FileMgr::BuildFileList(root, &fileList, &PackFileFilter);
	
	const FileReference *reference = fileList.First();
	while (reference)
	{
		ResourcePath path(GetRootPath());
		const char *name = reference->GetName();
		path += name;
		
		PackFile *packFile = new PackFile(name);
		if (packFile->Open(path) == kResourceOkay)
		{
			PackFile *prevFile = packFileList.First();
			if (prevFile)
			{
				do
				{
					if (prevFile->GetLocationPath() < packFile->GetLocationPath())
					{
						packFileList.InsertBefore(packFile, prevFile);
						goto next;
					}

					prevFile = prevFile->Next();
				} while (prevFile);
			}
			
			packFileList.Append(packFile);
		}
		else
		{
			delete packFile;
		}

		next:
		reference = reference->Next();
	}
	
	BuildDirectoryList();
}

VirtualResourceCatalog::~VirtualResourceCatalog()
{
}

bool VirtualResourceCatalog::PackFileFilter(const char *name, unsigned_int32 flags, void *cookie)
{
	if ((flags & kFileDirectory) || (name[0] == '.')) return (false);
	
	int32 len = Text::GetTextLength(name);
	return ((len > 4) && (name[len - 4] == '.') && (name[len - 3] == 'p') && (name[len - 2] == 'a') && (name[len - 1] == 'k'));
}

void VirtualResourceCatalog::BuildDirectoryList(void)
{
	directoryList.Purge();
	FileMgr::BuildFileList(GetRootPath(), &directoryList, &FileMgr::DirectoryFilter);
}

ResourceResult VirtualResourceCatalog::OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, const char *name, ResourceLocation *location)
{
	ResourceType type = descriptor->GetType();
	
	if (!(flags & kResourceIgnorePackFiles))
	{
		PackFile *packFile = packFileList.First();
		while (packFile)
		{
			const PackFileEntry *entry = packFile->FindResource(type, name);
			if (entry)
			{
				loader->Open(packFile->GetFile(), packFile->GetDataStart() + (entry->dataStart << 4), entry->dataSize);
				if (location) location->GetPath() = packFile->GetLocationPath();
				return (kResourceOkay);
			}
			
			packFile = packFile->Next();
		}
	}
	
	ResourcePath path(GetRootPath());
	
	char x = (char) (type >> 16);
	char y = (char) ((type >> 8) & 0xFF);
	char z = (char) (type & 0xFF);
	
	const FileReference *directory = directoryList.First();
	while (directory)
	{
		int32 rootLen = GetRootPathLength();
		int32 len = Text::CopyText(directory->GetName(), &path[rootLen], kMaxResourcePathLength - 5 - rootLen) + rootLen;
		path[len++] = '/';
		len += Text::CopyText(name, &path[len], kMaxResourcePathLength - 4 - len);
		
		if (!(descriptor->GetFlags() & kResourceDontAppendType))
		{
			path[len++] = '.';
			path[len++] = x;
			path[len++] = y;
			path[len++] = z;
			path[len] = 0;
		}
		
		if (loader->Open(path) == kResourceOkay)
		{
			if (location) location->GetPath() = directory->GetName();
			return (kResourceOkay);
		}
		
		directory = directory->Next();
	}
	
	return (kResourceNotFound);
}

void VirtualResourceCatalog::CloseLoader(ResourceLoader *loader)
{
	loader->Close();
}

void VirtualResourceCatalog::BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const
{
	ResourceType type = descriptor->GetType();
	
	const FileReference *directory = directoryList.First();
	while (directory)
	{
		ResourcePath path(GetRootPath());
		path += directory->GetName();
		if (subdir)
		{
			path += '/';
			path += subdir;
		}
		
		FileMgr::BuildFileList(path, list, &ResourceFilter, &type);
		
		directory = directory->Next();
	}
}


ResourceMgr::ResourceMgr(int)
{
}

ResourceMgr::~ResourceMgr()
{
}

EngineResult ResourceMgr::Construct(void)
{
	genericCatalog.Construct("../Data/");
	virtualCatalog.Construct("../Data/");
	
	saveCatalogPath = "Save/";
	systemCatalogPath = "";
	configCatalogPath = "Engine/";
	
	String<MAX_PATH>	path;
		
	if (SHGetFolderPathA(nullptr, CSIDL_PERSONAL | CSIDL_FLAG_CREATE, nullptr, 0, path) == S_OK)
	{
		(path += '/') += TheEngine->GetApplicationName();
		if (FileMgr::CreateDirectory(path) == kFileOkay)
			(saveCatalogPath = path) += '/';
	}
		
	if (SHGetFolderPathA(nullptr, CSIDL_LOCAL_APPDATA | CSIDL_FLAG_CREATE, nullptr, 0, path) == S_OK)
	{
		(path += '/') += TheEngine->GetApplicationName();
		if (FileMgr::CreateDirectory(path) == kFileOkay)
			(systemCatalogPath = path) += '/';
	}
		
	if (SHGetFolderPathA(nullptr, CSIDL_APPDATA | CSIDL_FLAG_CREATE, nullptr, 0, path) == S_OK)
	{
		(path += '/') += TheEngine->GetApplicationName();
		if (FileMgr::CreateDirectory(path) == kFileOkay)
			(configCatalogPath = path) += '/';
	}
		
	defaultSaveCatalog.Construct(saveCatalogPath);
	currentSaveCatalog = defaultSaveCatalog;
	
	systemCatalog.Construct(systemCatalogPath);
	configCatalog.Construct(configCatalogPath);
	
	#if C4LOG_RESOURCES
	
		resourceLog.Open("Resources.txt", kFileCreate);
	
	#endif
	
	return (kEngineOkay);
}

void ResourceMgr::Destruct(void)
{
	#if C4LOG_RESOURCES
	
		resourceLog.Close();
	
	#endif
	
	configCatalog.Destruct();
	systemCatalog.Destruct();
	defaultSaveCatalog.Destruct();
	virtualCatalog.Destruct();
	genericCatalog.Destruct();
}

ResourceResult ResourceMgr::LoadResource(ResourceBase *resource, const ResourceDescriptor *descriptor, unsigned_int32 flags, ResourceLocation *location)
{
	ResourceLoader	loader;
	
	ResourceResult result = resource->OpenLoader(&loader, descriptor, flags, location);
	if (result == kResourceOkay)
	{
		result = resource->Load(&loader);
		resource->CloseLoader(&loader);
	}
	
	return (result);
}

Variable *ResourceMgr::FindVariableName(const char *name, int32 *length, int32 *restart)
{
	for (machine x = 0;;)
	{
		int32 p = Text::FindChar(&name[x], '(');
		if (p < 0) break;
		
		p += x;
		x++;
		
		if (name[x] == '$')
		{
			x++;
			int32 q = Text::FindChar(&name[x], ')');
			if (q >= 0)
			{
				ResourceName	variableName;
				
				variableName.Set(&name[x], q);
				x += q + 1;
				
				Variable *variable = TheEngine->GetVariable(variableName);
				if (variable)
				{
					*length = p;
					*restart = x;
					return (variable);
				}
			}
		}
	}
	
	return (nullptr);
}

ResourceBase *ResourceMgr::GetResource(const ResourceDescriptor *descriptor, ResourceCatalog *catalog, const char *name, unsigned_int32 flags, ResourceLocation *location, ResourceBase::NewProc *newProc)
{
	int32			length;
	int32			restart;
	ResourceName	modifiedName;
	
	const char *finalName = name;
	
	Variable *variable = FindVariableName(name, &length, &restart);
	if (variable)
	{
		modifiedName.Set(name, length);
		for (;;)
		{
			modifiedName += variable->GetValue();
			name += restart;
			
			variable = FindVariableName(name, &length, &restart);
			if (!variable)
			{
				modifiedName += name;
				break;
			}
			
			modifiedName.Append(name, length);
		}
			
		finalName = modifiedName;
	}
	
	if (!catalog) catalog = virtualCatalog;
	
	ResourceTracker *tracker = catalog->GetTracker(descriptor);
	ResourceBase *resource = tracker->FindResource(finalName);
	if (resource)
	{
		if ((!resource->GetData()) && (!(flags & kResourceDeferLoad)))
		{
			if (LoadResource(resource, descriptor, flags, location) != kResourceOkay)
			{	
				resource->Shared::Release();
				return (nullptr);
			}
		}
		
		tracker->RetainResource(resource);
		return (resource);
	}
	
	#if C4LOG_RESOURCES
	
		ResourcePath path(finalName, kMaxResourcePathLength - 5);
		int32 len = path.Length();
		if (len != 0)
		{
			if (!(descriptor->GetFlags() & kResourceDontAppendType))
			{
				ResourceType type = descriptor->GetType();
				path[len++] = '.';
				path[len++] = (char) (type >> 16);
				path[len++] = (char) ((type >> 8) & 0xFF);
				path[len++] = (char) (type & 0xFF);
			}
			
			path[len++] = '\n';
			path[len] = 0;
			
			resourceLog << path;
		}
	
	#endif
	
	resource = (*newProc)(finalName, catalog);
	
	if (!(flags & kResourceDeferLoad))
	{
		if (LoadResource(resource, descriptor, flags, location) != kResourceOkay)
		{	
			resource->Shared::Release();
			return (nullptr);
		}
	}
	
	tracker->AddResource(resource);
	return (resource);
}

void ResourceMgr::FlushCache(const ResourceDescriptor *descriptor, ResourceCatalog *catalog)
{
	if (!catalog) catalog = virtualCatalog;
	
	ResourceType type = descriptor->GetType();
	ResourceTracker *tracker = catalog->GetTrackerMap()->Find(type);
	if (tracker) tracker->FlushCache();
}

FileResult ResourceMgr::CreateDirectoryPath(const char *path)
{
	FileResult result = FileMgr::CreateDirectoryPath(path);
	if (result == kFileOkay) virtualCatalog->BuildDirectoryList();
	return (result);
}

// ZYURVUR
