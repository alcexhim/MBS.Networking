using System;
using System.Collections.Generic;

namespace MBS.Networking
{
	public abstract class TransportServer
	{
		/// <summary>
		/// The TransportServer is stopping base on user request (i.e. Stop() method)
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnStopping(System.ComponentModel.CancelEventArgs e)
		{
		}
		/// <summary>
		/// The TransportServer has been successfully stopped by user request.
		/// </summary>
		/// <param name="e">E.</param>
		protected virtual void OnStopped(EventArgs e)
		{
		}

		public void Stop()
		{
			System.ComponentModel.CancelEventArgs e = new System.ComponentModel.CancelEventArgs();
			OnStopping(e);
			if (e.Cancel) return;

			foreach (System.Threading.Thread thread in threads.Values)
			{
				thread.Abort();
			}
			threads.Clear();

			OnStopped(EventArgs.Empty);
		}

		/// <summary>
		/// Causes the underlying implementation to start
		/// </summary>
		protected abstract void StartInternal();

		/// <summary>
		/// Causes the underlying implementation to listen for request
		/// </summary>
		/// <returns>The request internal.</returns>
		protected abstract TransportClient AcceptClientInternal();

		public void Start()
		{
			StartInternal();

			running = true;
			while (running)
			{
				TransportClient client = AcceptClientInternal();

				// handle the newly-connected client on a new thread
				threads[client] = new System.Threading.Thread(thread_ThreadStart);
				threads[client].Start(client);
			}
		}

		public event TransportClientConnectedEventHandler ClientConnected;
		protected virtual void OnClientConnected(TransportClientConnectedEventArgs e)
		{
			ClientConnected?.Invoke(this, e);
		}


		private Dictionary<TransportClient, System.Threading.Thread> threads = new Dictionary<TransportClient, System.Threading.Thread>();
		private bool running = false;
		private void thread_ThreadStart(object param)
		{
			TransportClient client = (param as TransportClient);

			// TODO: handle this
			OnClientConnected(new TransportClientConnectedEventArgs(client));
		}
	}
}
