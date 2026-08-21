// device_info_manager.cpp : implementation file
//

#include "stdafx.h"
#include "device_info_manager.h"
#include "control/device_tree.h"

using namespace client;

//////////////////////////////////////////////////////////////////////////

const wchar_t* const DAT_FILE = L"Tester_G2ClientGDK_RAS.dat";

//////////////////////////////////////////////////////////////////////////

device_info_manager::device_info_manager(void)
{
    import_from_file();
}


device_info_manager::~device_info_manager(void)
{
    export_to_file();
}

//////////////////////////////////////////////////////////////////////////

void device_info_manager::import_from_file(void)
{
    CString strFolderPath;
    ::GetModuleFileName(NULL, strFolderPath.GetBuffer(MAX_PATH), MAX_PATH);
    strFolderPath.ReleaseBuffer();
    if (strFolderPath.Find('\\') != -1) {
        for (int i = strFolderPath.GetLength() - 1; i >= 0; i--) {
            TCHAR ch = strFolderPath[i];
            if (ch == '\\') break; 
            strFolderPath.Delete(i);
        }
    }
    strFolderPath += DAT_FILE;

    CFile file;
    if (file.Open(strFolderPath, CFile::modeRead)) {
        CArchive ar(&file, CArchive::load);
        unsigned int size;
        bool isClear = false;

        try {
            ar >> size;
            for (unsigned int i = 0; i < size; ++i) {
                device_info data;
                ar >> data._site;
                ar >> data._address;
                ar >> data._id;
                ar >> data._password;
                ar >> data._adminPort;
                ar >> data._watchPort;
                ar >> data._searchPort;
                ar >> data._audioPort;
                if (data._site.IsEmpty() == false) _repo.add(data);
            }
        }
        catch (CFileException *fe) {
            fe->ReportError();
            isClear = true;
        }
        catch (CArchiveException *ae) {
            ae->ReportError();
            isClear = true;
        }

        ar.Close();
        file.Close();

        if (isClear) {
            _repo.clear();
            CFile::Remove(strFolderPath);
        }
    }
}

void device_info_manager::export_to_file(void)
{
    CString strFolderPath;
    ::GetModuleFileName(NULL, strFolderPath.GetBuffer(MAX_PATH), MAX_PATH);
    strFolderPath.ReleaseBuffer();
    if (strFolderPath.Find('\\') != -1) {
        for (int i = strFolderPath.GetLength() - 1; i >= 0; i--) {
            TCHAR ch = strFolderPath[i];
            if (ch == '\\') break; 
            strFolderPath.Delete(i);
        }
    }
    strFolderPath += DAT_FILE;

    CFile file;
    if (file.Open(strFolderPath, CFile::modeCreate | CFile::modeWrite)) {
        CArchive ar(&file, CArchive::store);
        bool isRemove = false;
        
        try {
            ar << _repo.size();
            for (iterator itr = _repo.begin();
                itr != _repo.end();
                ++itr) {
                device_info data = *itr;
                ar << data._site;
                ar << data._address;
                ar << data._id;
                ar << data._password;
                ar << data._adminPort;
                ar << data._watchPort;
                ar << data._searchPort;
                ar << data._audioPort;
            }
        }
        catch (CFileException *fe) {            
            fe->ReportError();
            isRemove = true;
        }
        catch (CArchiveException *ae) {
            ae->ReportError();
            isRemove = true;
        }

        ar.Close();
        file.Close();

        if (isRemove) {
            CFile::Remove(strFolderPath);
        }
    }
}

//////////////////////////////////////////////////////////////////////////

bool device_info_manager::add(const valueType& data, bool isDvr)
{
    if (_tree.get() == NULL) return false;

    return data._site.IsEmpty() == false && 
           _repo.add(data) && 
           _tree->append_device(data._site, isDvr);
}

bool device_info_manager::add_child(const keyType& site, const CString& child, int channel, bool isCamera)
{
    if (_tree.get() == NULL) return false;

    valueType data;
    if (get_info(site, data)) {
        return child.IsEmpty() == false &&
               _tree->append_device_child(site, child, channel, isCamera);
    }
    return false;
}

bool device_info_manager::remove_child(const keyType& site, int channel)
{
    if (_tree.get() == NULL) return false;

    valueType data;
    if (get_info(site, data)) {
        return _tree->remove_device_child(site, channel);
    }
    return false;
}

bool device_info_manager::remove_children(const keyType& site)
{
    if (_tree.get() == NULL) return false;

    valueType data;
    if (get_info(site, data)) {
        return _tree->remove_device_children(site);
    }
    return false;
}

bool device_info_manager::modify_child(const keyType& site, const CString& child, int channel, bool isCamera)
{
    return add_child(site, child, channel, isCamera);
}

bool device_info_manager::remove_selected()
{
    if (_tree.get() == NULL) return false;

    CString selected = _tree->selected_device();
    return selected.IsEmpty() == false &&
           _repo.remove(selected) &&
           _tree->remove_device(selected);
}

bool device_info_manager::modify(const keyType& key, const valueType& data)
{
    if (_tree.get() == NULL) return false;

    return key.IsEmpty() == false &&
           data._site.IsEmpty() == false &&
           _repo.modify(key, data) &&
           _tree->modify_device(key, data._site);
}

bool device_info_manager::get_info(const keyType& key, valueType& data)
{
    iterator findData = _repo.find(key);
    bool retv = findData != _repo.end();
    if (retv) { data = *findData; }
    return retv;
}

bool device_info_manager::get_child_info(const keyType& key, int channel, CString& text)
{
    if (_tree.get() == NULL) return false;
    return _tree->get_child_text(key, channel, text);;
}

CString device_info_manager::selected_site()
{
    return _tree.get() ? _tree->selected_device() : CString();
}

bool device_info_manager::has_children(const keyType& key)
{
    return _tree.get() ? _tree->has_children(key) : false;
}

void device_info_manager::update(void)
{
    if (_tree.get() == NULL) return;
    _tree->clear();

    for (iterator itr = _repo.begin();
        itr != _repo.end();
        ++itr) {
        device_info info = *itr;
        _tree->append_device(info._site);
    }
}

void device_info_manager::clear(void)
{
    if (_tree.get() == NULL) return;
    _repo.clear();
    _tree->clear();
}