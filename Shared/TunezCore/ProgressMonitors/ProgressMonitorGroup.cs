using System;
using System.Collections;
using System.Collections.Generic;

namespace Tunez
{
	public class TrackMonitorGroup : TrackMonitor, IEnumerable <TrackMonitor>
	{
		List<TrackMonitor> monitors = new List<TrackMonitor> ();

		public void Add(TrackMonitor monitor)
		{
			monitors.Add(monitor);
		}

		public override void Initialize (Track track)
		{
			base.Initialize (track);
			foreach (var monitor in monitors)
				monitor.Initialize (track);
		}

		protected override void OnReport(TimeSpan value)
		{
			foreach (IProgress<TimeSpan> monitor in monitors)
				monitor.Report (value);
		}

		public IEnumerator<TrackMonitor> GetEnumerator()
		{
			return monitors.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator ();
		}
	}
}
