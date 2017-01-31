﻿/*
    _                _      _  ____   _                           _____
   / \    _ __  ___ | |__  (_)/ ___| | |_  ___   __ _  _ __ ___  |  ___|__ _  _ __  _ __ ___
  / _ \  | '__|/ __|| '_ \ | |\___ \ | __|/ _ \ / _` || '_ ` _ \ | |_  / _` || '__|| '_ ` _ \
 / ___ \ | |  | (__ | | | || | ___) || |_|  __/| (_| || | | | | ||  _|| (_| || |   | | | | | |
/_/   \_\|_|   \___||_| |_||_||____/  \__|\___| \__,_||_| |_| |_||_|   \__,_||_|   |_| |_| |_|

 Copyright 2015-2017 Łukasz "JustArchi" Domeradzki
 Contact: JustArchi@JustArchi.net

 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at

 http://www.apache.org/licenses/LICENSE-2.0
					
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.

*/

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ArchiSteamFarm {
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[Target("Steam")]
	internal sealed class SteamTarget : TargetWithLayout {
		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		// This is NLog config property, it must have public get() and set() capabilities
		public string BotName { get; set; }

		[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
		[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
		[RequiredParameter]
		// This is NLog config property, it must have public get() and set() capabilities
		public ulong SteamID { get; set; }

		[SuppressMessage("ReSharper", "EmptyConstructor")]
		public SteamTarget() {
			// This constructor is intentionally empty and public, as NLog uses it for creating targets
			// It must stay like this as we want to have SteamTargets defined in our NLog.config
		}

		protected override void Write(LogEventInfo logEvent) {
			if (logEvent == null) {
				ASF.ArchiLogger.LogNullError(nameof(logEvent));
				return;
			}

			if (SteamID == 0) {
				return;
			}

			Bot bot;
			if (string.IsNullOrEmpty(BotName)) {
				bot = Bot.Bots.Values.FirstOrDefault(targetBot => targetBot.IsConnectedAndLoggedOn && targetBot.IsFriend(SteamID));
				if (bot == null) {
					return;
				}
			} else {
				if (!Bot.Bots.TryGetValue(BotName, out bot)) {
					return;
				}
			}

			string message = Layout.Render(logEvent);
			if (string.IsNullOrEmpty(message)) {
				return;
			}

			bot.SendMessage(SteamID, message);
		}
	}
}