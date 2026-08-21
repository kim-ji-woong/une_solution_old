// g2_backup.h : header file
//

#ifndef _G2_CLIENT_DLL_BACKUP_SAVER_H_
#define _G2_CLIENT_DLL_BACKUP_SAVER_H_

#if _MSC_VER > 1000
#pragma  once
#endif // _MSC_VER > 1000

#include "g2_define_backup_saver.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC void G2API g2_backup_saver_register_callback(G2HBACKUP_SAVER handle, unsigned int type, G2FUN_LISTENER func);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC G2HBACKUP G2API g2_backup_saver_initialize(G2UPARAM param);
G2_DLLFUNC void G2API g2_backup_saver_finalize(G2HBACKUP_SAVER handle);
G2_DLLFUNC void G2API g2_backup_saver_startup(G2HBACKUP_SAVER handle, int connections);
G2_DLLFUNC void G2API g2_backup_saver_cleanup(G2HBACKUP_SAVER handle);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC int  G2API g2_backup_saver_connect(G2HBACKUP_SAVER handle, const G2GUID* service, const G2GUID* site, const G2CONNECT_OPTIONS* options, G2CONNECT_RES* res);
G2_DLLFUNC void G2API g2_backup_saver_disconnect(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_is_connecting(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_is_connected(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_is_disconnecting(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_is_disconnected(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_is_disconnectable(G2HBACKUP_SAVER handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_saver_set_camera_list(G2HBACKUP_SAVER handle, int channel, const G2CHANNEL_SET* channels, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_backup_saver_set_camera_list_interest(G2HBACKUP_SAVER handle, int channel, const G2CHANNEL_SET* channels);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_saver_request_backup_site(G2HBACKUP_SAVER handler, int channel, const G2GUID* siteGUID);
G2_DLLFUNC bool G2API g2_backup_saver_request_record_channels(G2HBACKUP_SAVER handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_backup_saver_request_query_no_recorded_data(G2HBACKUP_SAVER handle, int channel, const G2GUID camera[], unsigned int count);
G2_DLLFUNC bool G2API g2_backup_saver_request_play(G2HBACKUP_SAVER handle, int channel, const G2PLAYBACK_COMMAND* command);
G2_DLLFUNC bool G2API g2_backup_saver_request_pause(G2HBACKUP_SAVER handle, int channel, bool rollback, const G2ROLLBACK_INFO* rbi);
G2_DLLFUNC bool G2API g2_backup_saver_request_move_to_spot(G2HBACKUP_SAVER handle, int channel, const G2SPOT* spot, int precision, bool forward);
G2_DLLFUNC bool G2API g2_backup_saver_request_notify_end_of_play(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_request_scope_list(G2HBACKUP_SAVER handle, int channel, const G2TIME* from, const G2TIME* to, const G2CHANNEL_SET* channels);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_measure_size(G2HBACKUP_SAVER handle, int channel, const G2CHANNEL_SET* channels, const G2SCOPE* scope, unsigned int free_space);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_password(G2HBACKUP_SAVER handle, int channel, const wchar_t* password);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_text_in(G2HBACKUP_SAVER handle, int channel, bool include);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_cancel(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_size(G2HBACKUP_SAVER handle, int channel);
G2_DLLFUNC bool G2API g2_backup_saver_request_clipcopy_data(G2HBACKUP_SAVER handle, int channel);

//////////////////////////////////////////////////////////////////////////

G2_DLLFUNC bool G2API g2_backup_saver_get_clipcopy_size_info(G2HBACKUP_SAVER handle, int channel, G2CLIPCOPY_SIZE_INFO* csi);

//////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_CLIENT_DLL_PLAY_SAVER_H_
