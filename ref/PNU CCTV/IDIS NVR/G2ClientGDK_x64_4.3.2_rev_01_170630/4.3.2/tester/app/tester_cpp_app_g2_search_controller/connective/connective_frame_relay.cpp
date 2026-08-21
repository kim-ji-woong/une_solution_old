// connective_frame_relay.cpp : implementation file
//

#include "stdafx.h"
#include "G2SearchControllerView.h"

#include <sampler/cpp/app/g2app_frame_relay.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

using namespace client;

//////////////////////////////////////////////////////////////////////////

void G2SearchControllerView::on_g2app_frame_relay_connected(G2HANDLE handle, int channel)
{
    PostMessage(um_runner_::UM_CONNECTED_RELAY);
}

void G2SearchControllerView::on_g2app_frame_relay_disconnected(G2HANDLE handle, int channel, int reason)
{
    G2RETURN_IF_FAIL(_relay->safe_handle() == handle);
    
    PostMessage(um_runner_::UM_DISCONNECTED_RELAY);
}

void G2SearchControllerView::on_g2app_frame_relay_receive_request_site_product_info(G2HANDLE handle, int channel)
{

}
