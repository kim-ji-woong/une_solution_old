using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SensorMonitor
{
	public class IPSerial
	{
		public const int NSIO_OK = 0;
		/*----------------------------------------------------------------------*/
		/*	Parameters for nsio_ioctl function 									*/	
		/*----------------------------------------------------------------------*/
		public const int B50 = 				0;	/* IOCTL : baud rate = 50			*/
		public const int B75 =				1;	/* IOCTL : baud rate = 75			*/
		public const int B110 =				2;	/* IOCTL : baud rate = 110			*/
		public const int B134 =				3;	/* IOCTL : baud rate = 134.5		*/
		public const int B150 =				4;	/* IOCTL : baud rate = 150			*/
		public const int B300 =				5;	/* IOCTL : baud rate = 300 bps		*/
		public const int B600 =				6;	/* IOCTL : baud rate = 600 bps		*/
		public const int B1200 =				7;	/* IOCTL : baud rate = 1200 bps		*/
		public const int B2400 =				9;	/* IOCTL : baud rate = 2400 bps		*/
		public const int B4800 =				10;	/* IOCTL : baud rate = 4800 bps		*/
		public const int B7200 =				11;	/* IOCTL : baud rate = 7200 bps		*/
		public const int B9600 =				12;	/* IOCTL : baud rate = 9600 bps		*/
		public const int B19200 =			13;	/* IOCTL : baud rate = 19200 bps	*/
		public const int B38400 =			14;	/* IOCTL : baud rate = 38400 bps	*/
		public const int B57600 =			15;	/* IOCTL : baud rate = 57600 bps	*/
		public const int B115200 = 			16;	/* IOCTL : baud rate = 115200 bps	*/
		public const int B230400 = 			17;	/* IOCTL : baud rate = 230400 bps	*/
		public const int B460800 =			18;	/* IOCTL : baud rate = 460800 bps	*/
		public const int B921600 =			19;	/* IOCTL : baud rate = 921600 bps	*/

		public const int BIT_8 =			3;	/* IOCTL : 8 data bits */
		public const int BIT_7 =			2;	/* IOCTL : 7 data bits */
		public const int BIT_6 =			1;	/* IOCTL : 6 data bits */
		public const int BIT_5 =			0;	/* IOCTL : 5 data bits */

		public const int STOP_1 =			0;	/* IOCTL : 1 stop bit  */
		public const int STOP_2 =			4;	/* IOCTL : 2/1.5 stop bits */

		/*----------------------------------------------------------------------*/
		/*	return value for lstatus function									*/	
		/*----------------------------------------------------------------------*/
		public const int S_CTS =				0x01;	/* line status : CTS on 	*/
		public const int S_DSR =				0x02;	/* line status : DSR on 	*/
		public const int S_DCD =				0x08;	/* line status : DCD on 	*/

		/* new define. Prevent define conflict with pcomm */
		public const int P_IP_SERIAL_NONE =				0;	/* IOCTL : none parity:0x0	*/
		public const int P_IP_SERIAL_EVEN =				8;	/* IOCTL : even parity:0x08	*/
		public const int P_IP_SERIAL_ODD =				16;	/* IOCTL : odd parity: 0x10	*/
		public const int P_IP_SERIAL_MARK =				24;	/* IOCTL : mark parity: 0x18	*/
		public const int P_IP_SERIAL_SPACE =			32;	/* IOCTL : space parity: 0x20 */

		/* If PComm.h is not include, this is for old compatible */
		public const int P_NONE =				P_IP_SERIAL_NONE;	/* IOCTL : none parity	*/
		public const int P_EVEN =				P_IP_SERIAL_EVEN;	/* IOCTL : even parity	*/
		public const int P_ODD =				P_IP_SERIAL_ODD;        /* IOCTL : odd parity	*/
		public const int P_MARK =				P_IP_SERIAL_MARK;	/* IOCTL : mark parity	*/
		public const int P_SPACE =				P_IP_SERIAL_SPACE;	/* IOCTL : space parity */

		public const int P_PCOMM_NONE =	0x00;
		public const int P_PCOMM_EVEN =	0x18;
		public const int P_PCOMM_ODD =	0x08;
		public const int P_PCOMM_MARK =	0x28;
		public const int P_PCOMM_SPACE=	0x38;
	


		/*----------------------------------------------------------------------*/
		/*	Parameters for nsio_flowctrl function								*/	
		/*----------------------------------------------------------------------*/
		public const int F_NONE =			0x00;	/* Flow Control : None 	*/
		public const int F_CTS =				0x01;	/* Flow Control : CTS 	*/
		public const int F_RTS =				0x02;	/* Flow Control : RTS 	*/
		public const int F_XON =				0x04;	/* Flow Control : XON  	*/
		public const int F_XOFF =			0x08;	/* Flow Control : XOFF 	*/
		public const int F_RTS_CTS =			F_RTS | F_CTS;
		public const int F_XON_XOFF=			F_XON | F_XOFF;
		public const int F_BOTH =			F_RTS | F_CTS | F_XON | F_XOFF;

		/*----------------------------------------------------------------------*/
		/*	Parameters for nsio_lctrl function									*/	
		/*----------------------------------------------------------------------*/
		public const int LCTRL_DTR =			0x01;	/* set DTR on		*/
		public const int LCTRL_RTS =			0x02;	/* set RTS on		*/

		/*----------------------------------------------------------------------*/
		/*	Parameters for D_COMMAND_FLUSH command								*/	
		/*----------------------------------------------------------------------*/
		public const int FLUSH_RXBUFFER =		0x00;	/* flush Rx buffer		*/
		public const int FLUSH_TXBUFFER	=		0x01;	/* flush Tx buffer		*/
		public const int FLUSH_ALLBUFFER =		0x02;	/* flush Rx & Tx buffer */


		[DllImport("IPSerial.dll")]
		public static extern int nsio_init();
		[DllImport("IPSerial.dll")]
		public static extern int nsio_end();

		[DllImport("IPSerial.dll")]
		public static extern int  nsio_resetserver(string server_ip, string password);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_checkalive(string server_ip, int timeout);

		// Port Control
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_open(string server_ip, int port_index, int timeouts);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_close(int port_id);
		
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_ioctl(int port_id, int baud, int mode);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_flowctrl(int port_id, int mode);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_flush(int port_id, int func);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_xonxoff_chars(int port_id, byte xon_char, byte xoff_char);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_DTR(int port_id, int mode);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_RTS(int port_id, int mode);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_lctrl(int port_id, int mode);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_set_xoff(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_set_xon(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_baud(int port_id, long speed);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_resetport(int port_id, string password);
		

		// Input/Output data
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_read(int port_id, byte[] buf, int len);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_SetReadTimeouts(int port_id, int timeouts);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_write(int port_id, string buf, int len);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_SetWriteTimeouts(int port_id, int timeouts);


		// Port Status Inquiry
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_lstatus(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern long nsio_iqueue(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern long nsio_oqueue(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_data_status(int port_id);

		// Miscellaneous
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_break(int port_id, int time);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_break_on(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_break_off(int port_id);
		[DllImport("IPSerial.dll")]
		public static extern int  nsio_breakcount(int port_id);



        public static int GetBuadrate(int nBuad)
        {
            switch(nBuad)
            {
                case 50:
                    return B50;
                case 75:
                    return B75;
                case 110:
                    return B110;
                case 134:
                    return B134;
                case 150:
                    return B150;
                case 300:
                    return B300;
                case 600:
                    return B600;
                case 1200:
                    return B1200;
                case 2400:
                    return B2400;
                case 4800:
                    return B4800;
                case 7200:
                    return B7200;
                case 9600:
                    return B9600;
                case 19200:
                    return B19200;
                case 38400:
                    return B38400;
                case 57600:
                    return B57600;
                case 115200:
                    return B115200;
                case 230400:
                    return B230400;
                case 460800:
                    return B460800;
                case 921600:
                    return B921600;
            };
            return -1;   	
        }
	}
}
