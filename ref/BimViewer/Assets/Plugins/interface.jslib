mergeInto(LibraryManager.library, {
	_OnFinishModelLoading : function () {
		OnFinishModelLoading();
  },
	_OnPoiClicked : function (id) {
		OnPoiClicked(Pointer_stringify(id));
  },
    _SendPoiList : function(json) {
		OnPoiListReceived(Pointer_stringify(json));
  },
});