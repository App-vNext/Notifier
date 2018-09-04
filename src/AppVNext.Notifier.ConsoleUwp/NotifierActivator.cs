// ******************************************************************
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

using DesktopNotifications;
using System;
using System.Runtime.InteropServices;
using static System.Console;
using static System.Environment;

namespace AppVNext.Notifier
{
	// This GUID should be unique and match the System.AppUserModel.ToastActivatorCLSID in the installer.
	[ClassInterface(ClassInterfaceType.None)]
	[ComSourceInterfaces(typeof(INotificationActivationCallback))]
	[Guid("4F5B934E-27C6-4AB6-A5EF-C3C71770E1A7"), ComVisible(true)]
	public class NotifierActivator : NotificationActivator
	{
		public override void OnActivated(string arguments, NotificationUserInput userInput, string appUserModelId)
		{
			if (arguments?.Length == 0)
			{
				WriteLine($"The user clicked on the toast.");
			}
			else
			{
				WriteLine($"The user clicked on the toast. {arguments}");
			}

			var inputs = string.Empty;
			if (userInput?.Data != null)
			{
				foreach (var input in userInput)
				{
					if (!string.IsNullOrEmpty(inputs))
					{
						inputs += "&";
					}
					inputs += $"{input.Key}={input.Value}";
				}
			}

			if (inputs != string.Empty)
			{
				WriteLine($"The user entered the following input values: {inputs}");
			}

			Exit(0);
		}
	}
}