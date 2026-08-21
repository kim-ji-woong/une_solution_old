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


#ifndef C4Resources_h
#define C4Resources_h


//# \component	Resource Manager
//# \prefix		ResourceMgr/


#include "C4Files.h"


#define C4LOG_RESOURCES		1


namespace C4
{
	typedef EngineResult ResourceResult;
	
	
	enum
	{
		kMaxResourceNameLength		= 95,
		kMaxResourcePathLength		= kMaxFilePathLength
	};
	
	
	typedef Type							ResourceType;
	typedef String<kMaxResourceNameLength>	ResourceName;
	typedef String<kMaxResourcePathLength>	ResourcePath;
	
	
	//# \enum	ResourceFlags
	
	enum
	{
		kResourceDontAppendType		= 1 << 0,		//## The three-character type is not be appended to resource names as a file extension.
		kResourceExtraByte			= 1 << 1		//## An extra byte is added to the end of the memory allocated for this type of resource.
	};
	
	
	//# \enum	LoadFlags
	
	enum
	{
		kResourceDeferLoad			= 1 << 0,		//## The resource data is not loaded when the resource object is constructed.
		kResourceNoDefault			= 1 << 1,		//## Do not load a default resource if the requested resource does not exist.
		kResourceIgnorePackFiles	= 1 << 2		//## Ignore pack files when loading the resource.
	};
	
	
	enum
	{
		kPackFileDefault			= 'C4C4'
	};
	
	
	enum
	{
		kResourceOkay				= kEngineOkay,
		kResourceNotFound			= (kManagerResource << 16) | 0x0001,
		kResourceLoadFailed			= (kManagerResource << 16) | 0x0002,
		kResourceInvalidPackFile	= (kManagerResource << 16) | 0x0003
	};
	
	
	struct PackDirectoryEntry;
	class ResourceTracker;
	class ResourceLoader;
	class ResourceCatalog;
	class Variable;
	
	
	struct PackHeader
	{
		int32			endian;
		int32			version;
		
		Type			packFileType;
		unsigned_int32	mapSize;
	};
	
	
	struct PackFileEntry
	{
		unsigned_int32	leftOffset;
		unsigned_int32	rightOffset;
		
		unsigned_int32	dataStart;
		unsigned_int32	dataSize;
		
		char			name[4];
		
		const PackFileEntry *GetLeftEntry(const void *base) const
		{
			return ((leftOffset != 0) ? reinterpret_cast<const PackFileEntry *>(static_cast<const char *>(base) + leftOffset) : nullptr);
		}
		
		const PackFileEntry *GetRightEntry(const void *base) const
		{
			return ((rightOffset != 0) ? reinterpret_cast<const PackFileEntry *>(static_cast<const char *>(base) + rightOffset) : nullptr);
		}
	}; 
	
	 
	struct PackTypeEntry 
	{ 
		unsigned_int32	leftOffset;
		unsigned_int32	rightOffset; 
		
		ResourceType	resourceType;
		unsigned_int32	fileRootOffset;
		 
		const PackTypeEntry *GetLeftEntry(const void *base) const
		{
			return ((leftOffset != 0) ? reinterpret_cast<const PackTypeEntry *>(static_cast<const char *>(base) + leftOffset) : nullptr);
		} 
		
		const PackTypeEntry *GetRightEntry(const void *base) const
		{
			return ((rightOffset != 0) ? reinterpret_cast<const PackTypeEntry *>(static_cast<const char *>(base) + rightOffset) : nullptr);
		}
		
		const PackFileEntry *GetFileRootEntry(const void *base) const
		{
			return ((fileRootOffset != 0) ? reinterpret_cast<const PackFileEntry *>(static_cast<const char *>(base) + fileRootOffset) : nullptr);
		}
	};
	
	
	struct PackDirectoryEntry
	{
		unsigned_int32	leftOffset;
		unsigned_int32	rightOffset;
		
		unsigned_int32	typeRootOffset;
		unsigned_int32	directoryRootOffset;
		
		char			name[4];
		
		const PackDirectoryEntry *GetLeftEntry(const void *base) const
		{
			return ((leftOffset != 0) ? reinterpret_cast<const PackDirectoryEntry *>(static_cast<const char *>(base) + leftOffset) : nullptr);
		}
		
		const PackDirectoryEntry *GetRightEntry(const void *base) const
		{
			return ((rightOffset != 0) ? reinterpret_cast<const PackDirectoryEntry *>(static_cast<const char *>(base) + rightOffset) : nullptr);
		}
		
		const PackTypeEntry *GetTypeRootEntry(const void *base) const
		{
			return ((typeRootOffset != 0) ? reinterpret_cast<const PackTypeEntry *>(static_cast<const char *>(base) + typeRootOffset) : nullptr);
		}
		
		const PackDirectoryEntry *GetDirectoryRootEntry(const void *base) const
		{
			return ((directoryRootOffset != 0) ? reinterpret_cast<const PackDirectoryEntry *>(static_cast<const char *>(base) + directoryRootOffset) : nullptr);
		}
	};
	
	
	class PackFile : public ListElement<PackFile>
	{
		private:
			
			File			packFile;
			ResourcePath	locationPath;
			
			char			*mapStorage;
			unsigned_int32	dataStart;
			
			const PackDirectoryEntry *GetRootDirectory(void) const
			{
				return (reinterpret_cast<const PackDirectoryEntry *>(mapStorage));
			}
			
			static int32 CompareFileNames(const char *name, const char *root);
			static int32 CompareDirectoryNames(const char *name, const char *root, int32 max);
			
			static void ReverseFileEntry(PackFileEntry *entry, const void *base);
			static void ReverseTypeEntry(PackTypeEntry *entry, const void *base);
			static void ReverseDirectoryEntry(PackDirectoryEntry *entry, const void *base);
			
			static const PackFileEntry *FindFileEntry(const PackTypeEntry *type, const void *base, const char *name);
			static const PackTypeEntry *FindTypeEntry(const PackDirectoryEntry *directory, const void *base, ResourceType type);
			static const PackDirectoryEntry *FindDirectoryEntry(const PackDirectoryEntry *directory, const void *base, const char *name, int32 len);
		
		public:
			
			PackFile(const char *fileName);
			~PackFile();
			
			ResourceResult Open(const char *name);
			
			File *GetFile(void)
			{
				return (&packFile);
			}
			
			const ResourcePath& GetLocationPath(void) const
			{
				return (locationPath);
			}
			
			unsigned_int32 GetDataStart(void) const
			{
				return (dataStart);
			}
			
			const PackFileEntry *FindResource(ResourceType type, const char *name) const;
	};
	
	
	//# \class	ResourceLocation	Stores the location of a resource.
	//
	//# \def	class ResourceLocation
	//
	//# \ctor	ResourceLocation();
	//
	//# \desc
	//# The $ResourceLocation$ class stores the location of a resource.
	//
	//# \also	$@Resource::Get@$
	//# \also	$@Resource::OpenLoader@$
	
	
	//# \function	ResourceLocation::GetPath	Returns the path stored in the resource location.
	//
	//# \proto	ResourcePath& GetPath(void);
	//# \proto	const ResourcePath& GetPath(void) const;
	//
	//# \desc
	//# The $GetPath$ function returns the path stored in the resource location.
	
	
	class ResourceLocation
	{
		private:
			
			ResourcePath	resourcePath;
		
		public:
			
			ResourceLocation()
			{
				resourcePath[0] = 0;
			}
			
			ResourcePath& GetPath(void)
			{
				return (resourcePath);
			}
			
			const ResourcePath& GetPath(void) const
			{
				return (resourcePath);
			}
	};
	
	
	class ResourceDescriptor
	{
		private:
			
			ResourceType	resourceType;
			char			resourceExtension[5];
			
			unsigned_int32	resourceFlags;
			unsigned_int32	resourceCacheSize;
			
			const char		*defaultResourceName;
		
		public:
			
			C4API ResourceDescriptor(const char *type, unsigned_int32 flags = 0, unsigned_int32 cacheSize = 0, const char *defaultName = "");
			
			ResourceType GetType(void) const
			{
				return (resourceType);
			}
			
			const char *GetExtension(void) const
			{
				return (resourceExtension);
			}
			
			unsigned_int32 GetFlags(void) const
			{
				return (resourceFlags);
			}
			
			unsigned_int32 GetCacheSize(void) const
			{
				return (resourceCacheSize);
			}
			
			const char *GetDefaultResourceName(void) const
			{
				return (defaultResourceName);
			}
			
			bool DescribesType(const char *type) const
			{
				return (resourceType == ((type[0] << 16) | (type[1] << 8) | type[2]));
			}
	};
	
	
	class C4_API ResourceBase : public HashTableElement<ResourceBase>, public ListElement<ResourceBase>, public Shared
	{
		friend class ResourceTracker;
		
		private:
			
			ResourceName		resourceName;
			unsigned_int32		resourceSize;
			char				*resourceData;
			
			ResourceCatalog		*resourceCatalog;
			ResourceTracker		*resourceTracker;
			
			virtual void Preprocess(void);
		
		protected:
			
			ResourceBase(const char *name, ResourceCatalog *catalog);
			~ResourceBase();
		
		public:
			
			typedef StringKey KeyType;
			typedef ResourceBase *NewProc(const char *, ResourceCatalog *);
			
			const KeyType GetKey(void) const
			{
				return (resourceName);
			}
			
			const ResourceName& GetName(void) const
			{
				return (resourceName);
			}
			
			unsigned_int32 GetSize(void) const
			{
				return (resourceSize);
			}
			
			void *GetData(void) const
			{
				return (resourceData);
			}
			
			int32 Release(void);
			
			static unsigned_int32 Hash(KeyType key);
			
			ResourceResult OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, ResourceLocation *location);
			void CloseLoader(ResourceLoader *loader);
			
			ResourceResult Load(ResourceLoader *loader);
	};
	
	
	//# \class	Resource	Encapsulates an individual data resource.
	//
	//# The $Resource$ class encapsulates an individual data resource.
	//
	//# \def	template <class type> class Resource : public ResourceBase
	//
	//# \tparam	type	The type of the $Resource$ subclass. This parameter should be the
	//#					type of the class that inherits directly from the $Resource$ class.
	//
	//# \ctor	Resource(const char *name, ResourceCatalog *catalog);
	//
	//# The constructor has protected access and can only be called as a base-class initializer for a subclass.
	//
	//# \param	name		The resource name.
	//# \param	catalog		The catalog through which the resource is loaded.
	//
	//# \desc
	//# The $Resource$ class template encapsulates an individual data resource of the type specified by the $type$
	//# template parameter. Each type of resource is represented by an object of a class type derived from the
	//# $Resource$ class template, where the $type$ template parameter is the same as the type of the subclass.
	//#
	//# A new resource is created by calling the $@Resource::Get@$ function. A resource is released by calling
	//# the $@Resource::Release@$ function. Resources are reference counted, so it is possible to acquire a
	//# pointer to the same resource object multiple times through calls to the $Get$ function. Each such call
	//# must be balanced by a matching call to the $Release$ function.
	//
	//# \privbase	ResourceBase	Used internally to encapsulate common functionality for all resource types.
	//
	//# \also	$@ResourceCatalog@$
	
	
	//# \function	Resource::Get	Returns a pointer to a resource object.
	//
	//# \proto	static type *Get(const char *name, unsigned_int32 flags = 0, ResourceCatalog *catalog = nullptr, ResourceLocation *location = nullptr);
	//
	//# \param	name		The name of the resource, including any containing folder names separated by forward slashes.
	//# \param	flags		The resource load flags. See below for possible values.
	//# \param	catalog		The resource catalog from which the resource should be loaded.
	//# \param	location	If not $nullptr$, then the path to the directory containing the resource is returned here when the resource is loaded from the virtual catalog.
	//
	//# \desc
	//# The $Get$ function returns a pointer to a resource object of the type corresponding to the template parameter
	//# of the $Resource$ class. If the resource object already exists, then its reference count is incremented and
	//# a pointer to the existing object is returned. Otherwise, a new resource object is created and the resource data
	//# is loaded into memory (unless the $kResourceDeferLoad$ flag is specified). If the resource data does not exist
	//# on disk, then the return value is $nullptr$.
	//#
	//# Each successful call to the $Get$ function should be balanced by a matching call to the $@Resource::Release@$
	//# function.
	//#
	//# The $flags$ parameter may be 0 or the following value.
	//
	//# \table	LoadFlags
	//
	//# If the $kResourceDeferLoad$ flag is specified, then the resource data is not loaded, but a resource object
	//# is created and returned. Note that in this case, the return value is never $nullptr$, so it's not possible
	//# to determine whether the resource data exists based on the return value. The resource data can subsequently
	//# be loaded outside of the resource object using the functions of the $@ResourceLoader@$ object returned
	//# by the $@Resource::OpenLoader@$ function. The resource data can also be loaded (in its entirety) into the
	//# $Resource$ object by calling the $@Resource::Load@$ function.
	//#
	//# If the $catalog$ parameter is $nullptr$, then the resource is loaded from the virtual catalog.
	//#
	//# When a resource is loaded from the virtual catalog and the $location$ parameter is not $nullptr$, then the
	//# path stored in the $@ResourceLocation@$ object is set to the path to the folder containing the resource file.
	//# If the resource was loaded from a pack file, then the path is set to that of the pack file without the extension.
	//# The resource location is not set if the $kResourceDeferLoad$ flag is specified in the $flags$ parameter.
	//
	//# \special
	//# The $Get$ function is normally not called directly using the $Resource$ class template. Higher-level classes
	//# normally make calls to the $Get$ function as part of a larger initialization procedure. The following list
	//# names some of those functions and the type of resources that they create.
	//
	//# \value	@SoundMgr/Sound::Load@				Sound resource.
	//# \value	@InterfaceMgr/Font::Get@			Font resource.
	//# \value	@GraphicsMgr/Texture::Get@			Texture resource.
	//# \value	@WorldMgr/Model::Get@				Model resource.
	//# \value	@WorldMgr/WorldMgr::LoadWorld@		World resource.
	//
	//# \special
	//# Resources indirectly loaded through these functions are released automatically when the higher-level classes
	//# that created them are themselves released through the proper calls.
	//
	//# \also	$@Resource::Load@$
	//# \also	$@Resource::Release@$
	//# \also	$@ResourceCatalog@$
	
	
	//# \function	Resource::Release	Releases a resource object.
	//
	//# \proto	int32 Release(void);
	//
	//# \desc
	//# The $Release$ function releases a resource object that was previously created with the $@Resource::Get@$
	//# function. The $Release$ function should be called to balance every call to the $Get$ function that does
	//# not return $nullptr$.
	//#
	//# The return value is the new reference count for the resource object after is has been released.
	//
	//# \also	$@Resource::Get@$
	
	
	//# \function	Resource::GetName	Returns the name of a resource.
	//
	//# \proto	const ResourceName& GetName(void) const;
	//
	//# \desc
	//# The $GetName$ function returns the name of a resource. This is the same name that was passed to the
	//# $@Resource::Get@$ function to create the resource object.
	//
	//# \also	$@Resource::GetSize@$
	//# \also	$@Resource::GetData@$
	
	
	//# \function	Resource::GetSize	Returns the size of a resource.
	//
	//# \proto	unsigned_int32 GetSize(void) const;
	//
	//# \desc
	//# The $GetSize$ function returns the size of a resource, in bytes. If the resource has not been loaded,
	//# then the returned size is zero.
	//
	//# \also	$@Resource::GetName@$
	//# \also	$@Resource::GetData@$
	
	
	//# \function	Resource::GetData	Returns a pointer to the data contained in a resource object.
	//
	//# \proto	void *GetData(void) const;
	//
	//# \desc
	//# The $GetData$ function returns a pointer to the data contained in a resource object. If the resource
	//# has not been loaded, then the returned size is $nullptr$.
	//
	//# \also	$@Resource::GetName@$
	//# \also	$@Resource::GetSize@$
	
	
	//# \div
	//# \function	Resource::OpenLoader	Opens a loader object for a particular resource.
	//
	//# \proto	ResourceResult OpenLoader(ResourceLoader *loader, ResourceLocation *location = nullptr);
	//
	//# \param	loader		A pointer to a newly constructed loader object.
	//# \param	location	If not $nullptr$, then the path to the directory containing the resource is returned here when the resource is loaded from the virtual catalog.
	//
	//# \desc
	//# The $OpenLoader$ function opens a $@ResourceLoader@$ object that can be used to read the data for
	//# a particular resource. Once the loader object is finished being used, it should be closed using the
	//# $@Resource::CloseLoader@$ function.
	//#
	//# If the loader object is successfully opened, then the return value is $kResourceOkay$. If the resource
	//# data does not exist, then the return value is $kResourceNotFound$.
	//#
	//# Once a loader object is opened, the functions of the $@ResourceLoader@$ class may be used to load
	//# resource data outside of the resource object. Alternatively, the $@Resource::Load@$ function may be
	//# used to load all of the resource data into the resource object.
	//
	//# When a resource is created using the virtual catalog and the $OpenLoader$ function is called with a $location$
	//# parameter that is not $nullptr$, then the path stored in the $@ResourceLocation@$ object is set to the path to
	//# the folder containing the resource file. If the resource was loaded from a pack file, then the path is set to
	//# that of the pack file without the extension.
	//
	//# \also	$@Resource::CloseLoader@$
	//# \also	$@Resource::Load@$
	//# \also	$@ResourceLoader@$
	
	
	//# \function	Resource::CloseLoader	Closes a loader object.
	//
	//# \proto	void CloseLoader(ResourceLoader *loader);
	//
	//# \param	loader		A pointer to an open loader object.
	//
	//# \desc
	//# The $CloseLoader$ function closes a loader object that was previously opened using the $@Resource::OpenLoader@$ function.
	//
	//# \also	$@Resource::OpenLoader@$
	//# \also	$@Resource::Load@$
	//# \also	$@ResourceLoader@$
	
	
	//# \function	Resource::Load		Loads the data for a resource object.
	//
	//# \proto	ResourceResult Load(ResourceLoader *loader);
	//
	//# \param	loader		An open loader object that can read the resource data.
	//
	//# \desc
	//# The $Load$ function loads the data for a resource object that was created by passing the $kResourceDeferLoad$
	//# flag to the $@Resource::Get@$ function. The $@ResourceLoader@$ object passed in the $loader$ parameter should
	//# have been opened by calling the $@Resource::OpenLoader@$ function for the same resource object for which the
	//# $Load$ function is called.
	//#
	//# If the resource data is successfully loaded, then the return value is $kResourceOkay$. Otherwise, the
	//# return value is $kResourceLoadFailed$.
	//#
	//# Once the resource data has been loaded into a resource object, the resource object owns the data and will
	//# release it properly. The $Load$ function should not be called more than once for the same resource object.
	//
	//# \also	$@Resource::OpenLoader@$
	//# \also	$@Resource::CloseLoader@$
	//# \also	$@Resource::Get@$
	//# \also	$@ResourceLoader@$
	
	
	template <class type> class Resource : public ResourceBase
	{
		private:
			
			static ResourceBase *New(const char *name, ResourceCatalog *catalog)
			{
				return (new type(name, catalog));
			}
		
		protected:
			
			Resource(const char *name, ResourceCatalog *catalog) : ResourceBase(name, catalog)
			{
			}
			
			~Resource()
			{
			}
		
		public:
			
			static const ResourceDescriptor *GetDescriptor(void)
			{
				return (&type::descriptor);
			}
			
			ResourceResult OpenLoader(ResourceLoader *loader, ResourceLocation *location = nullptr)
			{
				return (ResourceBase::OpenLoader(loader, GetDescriptor(), 0, location));
			}
			
			static type *Get(const char *name, unsigned_int32 flags = 0, ResourceCatalog *catalog = nullptr, ResourceLocation *location = nullptr);
	};
	
	
	class C4_API ResourceTracker : public MapElement<ResourceTracker>
	{
		private:
			
			ResourceType				resourceType;
			HashTable<ResourceBase>		resourceHashTable;
			
			unsigned_int32				maxCacheSize;
			unsigned_int32				currentCacheSize;
			List<ResourceBase>			cacheList;
		
		public:
			
			typedef ResourceType KeyType;
			
			ResourceTracker(const ResourceDescriptor *descriptor);
			~ResourceTracker();
			
			KeyType GetKey(void) const
			{
				return (resourceType);
			}
			
			ResourceType GetResourceType(void) const
			{
				return (resourceType);
			}
			
			const HashTable<ResourceBase> *GetHashTable(void) const
			{
				return (&resourceHashTable);
			}
			
			ResourceBase *FindResource(const char *name) const
			{
				return (resourceHashTable.Find(name));
			}
			
			unsigned_int32 GetCurrentCacheSize(void) const
			{
				return (currentCacheSize);
			}
			
			void FlushCache(void)
			{
				currentCacheSize = 0;
				cacheList.Purge();
			}
			
			void AddResource(ResourceBase *resource);
			int32 RetainResource(ResourceBase *resource);
			int32 ReleaseResource(ResourceBase *resource);
	};
	
	
	//# \class	ResourceLoader		Encapsulates functionality for loading resource data.
	//
	//# \def	class ResourceLoader
	//
	//# \ctor	ResourceLoader();
	//
	//# \desc
	//# The $ResourceLoader$ class encapsulates functionality for loading resource data outside of
	//# a resource object. Upon construction, a loader object is in the closed state. A loader object
	//# is opened by calling the $@Resource::OpenLoader@$ function for the resource object whose
	//# data is to be loaded through the loader object.
	//#
	//# Once a loader object is open, the size of the resource data can be obtained by calling the
	//# $@ResourceLoader::GetDataSize@$ function. Any part of the data itself can be read into memory
	//# using the $@ResourceLoader::Read@$ function. The resource data can be read into a resource object
	//# (in its entirety) by calling the $@Resource::Load@$ function.
	//
	//# \also	$@Resource::OpenLoader@$
	//# \also	$@Resource::CloseLoader@$
	//# \also	$@Resource::Load@$
	
	
	//# \function	ResourceLoader::GetDataSize		Returns the size of the resource data.
	//
	//# \proto	unsigned_int32 GetDataSize(void) const;
	//
	//# \desc
	//# The $GetDataSize$ function returns the size of the resource data, in bytes.
	//
	//# \also	$@ResourceLoader::Read@$
	
	
	//# \function	ResourceLoader::Read		Reads some or all of the resource data.
	//
	//# \proto	ResourceResult Read(void *buffer, unsigned_int32 start, unsigned_int32 size);
	//
	//# \param	buffer		A pointer to the memory location where the resource data is loaded.
	//# \param	start		The byte offset within the resource data at which the read operation starts.
	//# \param	size		The number of bytes that are read into memory.
	//
	//# \desc
	//# The $Read$ function reads a portion of the resource data into memory. This function reads $size$ bytes
	//# of data from the resource, beginning at the offset specified by the $start$ parameter, and stores them
	//# in memory beginning at the location specified by the $buffer$ parameter. The sum $start$ + $size$ should
	//# not exceed the size of the resource data returned by the $@ResourceLoader::GetDataSize@$ function.
	//#
	//# If the resource data is successfully loaded, then the return value is $kResourceOkay$. Otherwise, the
	//# return value is $kResourceLoadFailed$.
	//
	//# \also	$@ResourceLoader::GetDataSize@$
	
	
	class C4_API ResourceLoader
	{
		friend class ResourceBase;
		
		private:
			
			File			loaderFile;
			File			*resourceFile;
			
			unsigned_int32	dataStart;
			unsigned_int32	dataSize;
			unsigned_int32	memorySize;
		
		public:
			
			ResourceLoader();
			~ResourceLoader();
			
			unsigned_int32 GetDataSize(void) const
			{
				return (dataSize);
			}
			
			unsigned_int32 GetMemorySize(void) const
			{
				return (memorySize);
			}
			
			ResourceResult Open(const char *filename);
			void Open(File *file, unsigned_int32 start, unsigned_int32 size);
			void Close(void);
			
			ResourceResult Read(void *buffer, unsigned_int32 start, unsigned_int32 size);
	};
	
	
	//# \class	ResourceCatalog		Represents a storage repository from which resources can be loaded.
	//
	//# \def	class ResourceCatalog
	//
	//# \ctor	ResourceCatalog(const char *root);
	//
	//# The constructor has protected access. The $ResourceCatalog$ class can only exist as a base class for another class.
	//
	//# \param	root	The path to the root of the directory tree.
	//
	//# \desc
	//# 
	//
	//# \also	$@Resource@$
	
	
	class C4_API ResourceCatalog
	{
		private:
			
			const char				*rootPath;
			int32					rootPathLength;
			
			Map<ResourceTracker>	trackerMap;
		
		protected:
			
			ResourceCatalog(const char *root);
			
			static bool ResourceFilter(const char *name, unsigned_int32 flags, void *cookie);
		
		public:
			
			virtual ~ResourceCatalog();
			
			const char *GetRootPath(void) const
			{
				return (rootPath);
			}
			
			int32 GetRootPathLength(void) const
			{
				return (rootPathLength);
			}
			
			const Map<ResourceTracker> *GetTrackerMap(void) const
			{
				return (&trackerMap);
			}
			
			ResourceTracker *GetTracker(const ResourceDescriptor *descriptor);
			
			virtual ResourceResult OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, const char *name, ResourceLocation *location) = 0;
			virtual void CloseLoader(ResourceLoader *loader) = 0;
			
			virtual void BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const;
	};
	
	
	class C4_API GenericResourceCatalog : public ResourceCatalog
	{
		public:
			
			GenericResourceCatalog(const char *root);
			~GenericResourceCatalog();
			
			ResourceResult OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, const char *name, ResourceLocation *location);
			void CloseLoader(ResourceLoader *loader);
			
			void GetResourcePath(const ResourceDescriptor *descriptor, const char *name, ResourcePath *path) const;
			void GetResourcePath(const ResourceDescriptor *descriptor, const char *name, const ResourceLocation *location, ResourcePath *path) const;
			
			void BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const;
	};
	
	
	class C4_API VirtualResourceCatalog : public ResourceCatalog
	{
		friend class ResourceMgr;
		
		private:
			
			List<PackFile>			packFileList;
			List<FileReference>		directoryList;
			
			static bool PackFileFilter(const char *name, unsigned_int32 flags, void *cookie);
			
			void BuildDirectoryList(void);
		
		public:
			
			VirtualResourceCatalog(const char *root);
			~VirtualResourceCatalog();
			
			ResourceResult OpenLoader(ResourceLoader *loader, const ResourceDescriptor *descriptor, unsigned_int32 flags, const char *name, ResourceLocation *location);
			void CloseLoader(ResourceLoader *loader);
			
			void BuildResourceList(const ResourceDescriptor *descriptor, const char *subdir, List<FileReference> *list) const;
	};
	
	
	template <class type> class CatalogStorage
	{
		private:
			
			char		storage[sizeof(type)];
		
		public:
			
			type *Construct(const char *root)
			{
				return (new(storage) type(root));
			}
			
			void Destruct(void)
			{
				reinterpret_cast<type *>(storage)->~type();
			}
			
			operator type *(void)
			{
				return (reinterpret_cast<type *>(storage));
			}
			
			operator const type *(void) const
			{
				return (reinterpret_cast<const type *>(storage));
			}
			
			type *operator ->(void)
			{
				return (reinterpret_cast<type *>(storage));
			}
			
			const type *operator ->(void) const
			{
				return (reinterpret_cast<const type *>(storage));
			}
	};
	
	
	//# \class	ResourceMgr		Represents a storage repository from which resources can be loaded.
	//
	//# \def	class ResourceMgr : public Manager<ResourceMgr>
	//
	//# \desc
	//# The $ResourceMgr$ class encapsulates the resource loading functionality of the C4 Engine.
	//# The single instance of the Resource Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The Resource Manager's member functions are accessed through the global pointer $TheResourceMgr$.
	//
	//# \also	$@Resource@$
	//# \also	$@ResourceCatalog@$
	
	
	class C4_API ResourceMgr : public Manager<ResourceMgr>
	{
		template <class type> friend class Resource;
		
		private:
			
			GenericResourceCatalog						*currentSaveCatalog;
			
			CatalogStorage<GenericResourceCatalog>		genericCatalog;
			CatalogStorage<VirtualResourceCatalog>		virtualCatalog;
			
			CatalogStorage<GenericResourceCatalog>		defaultSaveCatalog;
			CatalogStorage<GenericResourceCatalog>		systemCatalog;
			CatalogStorage<GenericResourceCatalog>		configCatalog;
			
			String<kMaxFilePathLength>					saveCatalogPath;
			String<kMaxFilePathLength>					systemCatalogPath;
			String<kMaxFilePathLength>					configCatalogPath;
			
			#if C4LOG_RESOURCES
			
				File									resourceLog;
			
			#endif
			
			static Variable *FindVariableName(const char *name, int32 *length, int32 *restart);
			
			ResourceResult LoadResource(ResourceBase *resource, const ResourceDescriptor *descriptor, unsigned_int32 flags, ResourceLocation *location);
			ResourceBase *GetResource(const ResourceDescriptor *descriptor, ResourceCatalog *catalog, const char *name, unsigned_int32 flags, ResourceLocation *location, ResourceBase::NewProc *newProc);
		
		public:
			
			ResourceMgr(int);
			~ResourceMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			GenericResourceCatalog *GetGenericCatalog(void)
			{
				return (genericCatalog);
			}
			
			VirtualResourceCatalog *GetVirtualCatalog(void)
			{
				return (virtualCatalog);
			}
			
			GenericResourceCatalog *GetSaveCatalog(void) const
			{
				return (currentSaveCatalog);
			}
			
			void SetSaveCatalog(GenericResourceCatalog *catalog)
			{
				currentSaveCatalog = (catalog) ? catalog : defaultSaveCatalog;
			}
			
			GenericResourceCatalog *GetSystemCatalog(void)
			{
				return (systemCatalog);
			}
			
			GenericResourceCatalog *GetConfigCatalog(void)
			{
				return (configCatalog);
			}
			
			void FlushCache(const ResourceDescriptor *descriptor, ResourceCatalog *catalog = nullptr);
			
			FileResult CreateDirectoryPath(const char *path);
	};
	
	
	C4_API  extern ResourceMgr *TheResourceMgr;
	
	
	inline void ResourceBase::CloseLoader(ResourceLoader *loader)
	{
		resourceCatalog->CloseLoader(loader);
	}
	
	
	template <class type> inline type *Resource<type>::Get(const char *name, unsigned_int32 flags, ResourceCatalog *catalog, ResourceLocation *location)
	{
		return (static_cast<type *>(TheResourceMgr->GetResource(GetDescriptor(), catalog, name, flags, location, &New)));
	}
}


#endif

// ZYURVUR
