﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using IrcDotNet.Common.Collections;

namespace IrcDotNet
{
    /// <summary>
    /// Represents an IRC user that resides on a specific channel on a specific <see cref="IrcClient"/>.
    /// </summary>
    public class IrcChannelUser : INotifyPropertyChanged
    {
        // Internal and exposable collections of channel modes currently active on user.
        private HashSet<char> modes;
        private ReadOnlySet<char> modesReadOnly;

        private IrcChannel channel;
        private IrcUser user;

        internal IrcChannelUser(IrcUser user, IEnumerable<char> modes = null)
        {
            this.user = user;

            this.modes = new HashSet<char>();
            this.modesReadOnly = new ReadOnlySet<char>(this.modes);
            if (modes != null)
                this.modes.AddRange(modes);
        }

        /// <summary>
        /// A read-only collection of the channel modes the user currently has.
        /// </summary>
        /// <value>The current channel modes of the user.</value>
        public ReadOnlySet<char> Modes
        {
            get { return this.modesReadOnly; }
        }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public IrcChannel Channel
        {
            get { return this.channel; }
            internal set
            {
                this.channel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Channel"));
            }
        }

        /// <summary>
        /// Gets the <see cref="IrcUser"/> that is represented by the <see cref="IrcChannelUser"/>.
        /// </summary>
        /// <value>The <see cref="IrcUser"/> that is represented by the <see cref="IrcChannelUser"/>.</value>
        public IrcUser User
        {
            get { return this.user; }
        }

        /// <summary>
        /// Occurs when the channel modes of the user have changed.
        /// </summary>
        public event EventHandler<EventArgs> ModesChanged;
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void Kick(string comment = null)
        {
            this.channel.Client.Kick(new[] { this }, comment);
        }

        public void Op()
        {
            this.channel.SetModes("+o", this.user.NickName);
        }

        public void DeOp()
        {
            this.channel.SetModes("-o", this.user.NickName);
        }

        public void Voice()
        {
            this.channel.SetModes("+v", this.user.NickName);
        }

        public void DeVoice()
        {
            this.channel.SetModes("-v", this.user.NickName);
        }

        internal void HandleModeChanged(bool add, char mode)
        {
            if (add)
                this.modes.Add(mode);
            else
                this.modes.Remove(mode);
            OnModesChanged(new EventArgs());
        }

        /// <summary>
        /// Raises the <see cref="ModesChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnModesChanged(EventArgs e)
        {
            var handler = this.ModesChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            return this.channel.Name + "/" + this.user.NickName;
        }
    }
}
