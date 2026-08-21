// g2_define_backup.h : header file
//

#ifndef _G2_DEFINE_BACKUP_SAVER_H_
#define _G2_DEFINE_BACKUP_SAVER_H_

#if _MSC_VER > 1000
#pragma  once
#endif // _MSC_VER > 1000

#include "g2_define_backup.h"

#ifdef __cplusplus
extern "C" {
#endif

//////////////////////////////////////////////////////////////////////////

struct G2BACKUP_SAVER_CALLBACK {
    enum TYPE {
        on_connected = 0,
        on_disconnected = 1,
        on_receive_backup_site_result = 2,
        on_receive_record_channels = 3,
        on_receive_response_no_recorded_data = 4,
        on_receive_frame_data = 5,
        on_receive_notify_out_of_scope = 6,
        on_receive_notify_player_error = 7,
        on_receive_scope_list = 8,
        on_receive_no_recorded_data = 9,
        on_receive_clipcopy_size = 10,
        on_receive_clipcopy_data = 11,
        on_receive_clipcopy_canceled = 12,
        on_receive_clipcopy_set_password = 13,
        on_receive_clipcopy_job_started = 14,
        on_receive_clipcopy_job_finished = 15,

        CALLBACK_COUNT = 16
    };
};

///////////////////////////////////////////////////////////////////////////

#ifdef __cplusplus
}
#endif

#endif // !_G2_DEFINE_BACKUP_SAVER_H_
