/*
Objective:
Class of 3M message formatting
You can format a string message either in one step using FormatMessage(string)
or convert into encoded byte array, ConvertToCNS(string), first and then pad shift in/out, PadShift(byte[])


Last updated:
24 Feb 2004 by Chapman

C#.NET complier statement:
csc /t:library /out:..\bin\MMMFormatter.dll MMMFormatter.cs
*/

using System;
using System.Collections;
using System.Reflection;
using System.Text;

[assembly:AssemblyCompany("TDSL")]
[assembly:AssemblyCopyright("Assembly created on 2 Feb 2004. All rights reserved by TDSL.")]
[assembly:AssemblyDescription("Assembly contains methods to format message in 3M format")]
[assembly:AssemblyProduct("Sports.NET")]
[assembly:AssemblyTitle("PagerFormatter asembly")]
[assembly:AssemblyVersion("1.0.*")]
namespace PagerFormatter {
	public class MMMFormatter {
		//Constant
		private const byte CR = 0x0D;
		private const byte SHIFT_OUT = 0x0E;
		private const byte SHIFT_IN = 0x0F;
		private const byte DBBYTE_DELIMITER = 0x80;
		private const byte LINEFEED = 0x8A;
		private const byte HIGHLIGHT = 0x1F;
		private const int BIG5_CODEPAGE = 950;
		private const int CNS_CODEPAGE = 20000;		//CNS codepage is 20000 or 11643
		private const int PAGER_BYTEWIDTH = 20;

		//Parameters
		private Encoding BIG5Encoding;
		private Encoding CNSEncoding;
		private ArrayList m_EncodedList;

		//Constructor
		public MMMFormatter() {
			BIG5Encoding = Encoding.GetEncoding(BIG5_CODEPAGE);
			CNSEncoding = Encoding.GetEncoding(CNS_CODEPAGE);
			m_EncodedList = new ArrayList();
		}

		public byte ShiftOut {
			get {
				return SHIFT_OUT;
			}
		}

		public byte ShiftIn {
			get {
				return SHIFT_IN;
			}
		}

		public byte Linefeed {
			get {
				return LINEFEED;
			}
		}

		public byte HighLight {
			get {
				return HIGHLIGHT;
			}
		}

		public int PagerScreenWidth {
			get {
				return PAGER_BYTEWIDTH;
			}
		}

		//Format message in one step
		public byte[] FormatMessage(string sMessage) {
			byte[] resultBytes;
			try {
				resultBytes = (byte[])PadShift((byte[])ConvertToCNS(sMessage));
				
			} catch(Exception ex) {
				throw (new Exception("Format message error. " + ex.ToString()));
			}

			return resultBytes;
		}

		//Convert input string into CNS encoded byte array
		public byte[] ConvertToCNS(string sMessage) {
			byte[] encodedBytes;

			try {
				//Encode input message, by CNS codepage (20000 or 11643), into byte array
				encodedBytes = CNSEncoding.GetBytes(sMessage);
			} catch(Exception ex) {
				throw (new Exception("Encode message to CNS error. " + ex.ToString()));
			}

			return encodedBytes;
		}

		//Pad control byte into message byte array
		public byte[] PadShift(byte[] MessageBytes) {
			//Pad Shift-Out at starting point of double byte character
			//Check double byte: successive bytes (both 1st and follow byte) is greater than \x80
			////////////////////////////////////////////////////////////////////////////////////////
			//Pad Shift-In at starting point of single byte character
			//Check single byte: not double byte

			bool bDbByte = false;
			bool bSO_Padded = false;
			bool bSI_Padded = false;
			int iTotalLength = MessageBytes.Length;
			int iIdx = 0;
			byte[] resultBytes;
			m_EncodedList.Clear();

			try {
				while(iIdx < iTotalLength) {
					bDbByte = false;
					if(MessageBytes[iIdx].CompareTo(DBBYTE_DELIMITER) > 0) {	//Possible double-byte
						if((iTotalLength-iIdx) > 1) {	//Not last byte
							if(MessageBytes[iIdx+1].CompareTo(DBBYTE_DELIMITER) > 0) {	//Is a double-byte
								bDbByte = true;
							}
						}
					}

					if(bDbByte) {	//Pad shift-out if required
						if(!bSO_Padded) {
							m_EncodedList.Add(SHIFT_OUT);
							bSO_Padded = true;
						}
						m_EncodedList.Add(MessageBytes[iIdx]);
						m_EncodedList.Add(MessageBytes[iIdx+1]);
						bSI_Padded = false;
						iIdx++;
					} else {	//Pad shift-in if required
						if(!bSI_Padded) {
							m_EncodedList.Add(SHIFT_IN);
							bSI_Padded = true;
						}
						m_EncodedList.Add(MessageBytes[iIdx]);
						bSO_Padded = false;
					}
					iIdx++;
				}
				m_EncodedList.TrimToSize();

				//Unboxing return value of m_EncodedList.ToArray to original type, i.e. byte[]
				resultBytes = (byte[])m_EncodedList.ToArray(typeof(byte));
			} catch(Exception ex) {
				throw (new Exception("Pad shift character error. " + ex.ToString()));
			}

			return resultBytes;
		}

		public byte[] InsertPrefixGroup(string sPrefixGp, byte[] msgBytes) {
			byte[] resultBytes;
			byte[] prefixBytes = new byte[sPrefixGp.Length];
			try {
				int iIdx = 0;
				for(iIdx = 0; iIdx < sPrefixGp.Length; iIdx++) {
					switch(sPrefixGp.Substring(iIdx, 1)) {
						case "0":	prefixBytes[iIdx] = 0x90;
											break;
						case "1":	prefixBytes[iIdx] = 0x91;
											break;
						case "2":	prefixBytes[iIdx] = 0x92;
											break;
						case "3":	prefixBytes[iIdx] = 0x93;
											break;
						case "4":	prefixBytes[iIdx] = 0x94;
											break;
						case "5":	prefixBytes[iIdx] = 0x95;
											break;
						case "6":	prefixBytes[iIdx] = 0x96;
											break;
						case "7":	prefixBytes[iIdx] = 0x97;
											break;
						case "8":	prefixBytes[iIdx] = 0x98;
											break;
						case "9":	prefixBytes[iIdx] = 0x99;
											break;
						default:	prefixBytes[iIdx] = 0x99;
											break;
					}
				}
				
				m_EncodedList.Clear();
				m_EncodedList.AddRange(prefixBytes);
				m_EncodedList.AddRange(msgBytes);
				m_EncodedList.TrimToSize();

				//Unboxing return value of m_EncodedList.ToArray to original type, i.e. byte[]
				resultBytes = (byte[])m_EncodedList.ToArray(typeof(byte));
			} catch(Exception ex) {
				throw (new Exception("Insert prefix group from string error. " + ex.ToString()));
			}

			return resultBytes;
		}


		//Quote whole message content by HIGHLIGHT code in order to highlight the message
		//i.e. \x1F + messgae content + \x1F
		public byte[] HighlightMessage(byte[] inBytes) {
			byte[] resultBytes;
			m_EncodedList.Clear();

			try {
				m_EncodedList.Add(HIGHLIGHT);
				m_EncodedList.AddRange(inBytes);
				m_EncodedList.Add(HIGHLIGHT);
				m_EncodedList.TrimToSize();

				//Unboxing return value of m_EncodedList.ToArray to original type, i.e. byte[]
				resultBytes = (byte[])m_EncodedList.ToArray(typeof(byte));
			} catch(Exception ex) {
				throw (new Exception("Construct highlight message error. " + ex.ToString()));
			}

			return resultBytes;
		}

		//Insert header information, string format, at position 0
		//1st: value to insert
		//2nd: process byte array
		public byte[] InsertHeaderFromString(string sHeader, byte[] msgBytes) {
			byte[] encodedBytes;
			byte[] resultBytes;
			m_EncodedList.Clear();

			try {
				encodedBytes = BIG5Encoding.GetBytes(sHeader);
				m_EncodedList.AddRange(encodedBytes);
				m_EncodedList.AddRange(msgBytes);
				m_EncodedList.Add(CR);
				m_EncodedList.TrimToSize();

				//Unboxing return value of m_EncodedList.ToArray to original type, i.e. byte[]
				resultBytes = (byte[])m_EncodedList.ToArray(typeof(byte));
			} catch(Exception ex) {
				throw (new Exception("Insert header to message error. " + ex.ToString()));
			}

			return resultBytes;
		}

		public byte[] InsertHeaderWithShiftIn(string sHeader, byte[] msgBytes) {
			byte[] encodedBytes;
			byte[] resultBytes;
			m_EncodedList.Clear();

			try {
				encodedBytes = BIG5Encoding.GetBytes(sHeader);
				m_EncodedList.Add(SHIFT_IN);
				m_EncodedList.AddRange(encodedBytes);
				m_EncodedList.AddRange(msgBytes);
				m_EncodedList.TrimToSize();

				//Unboxing return value of m_EncodedList.ToArray to original type, i.e. byte[]
				resultBytes = (byte[])m_EncodedList.ToArray(typeof(byte));
			} catch(Exception ex) {
				throw (new Exception("Insert header with shift-in to message error. " + ex.ToString()));
			}

			return resultBytes;
		}
	}
}