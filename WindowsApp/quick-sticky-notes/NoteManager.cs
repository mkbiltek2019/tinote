﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace quick_sticky_notes
{
	public class NoteManager
	{
		private List<Note> notes = new List<Note>();

		public NoteManager()
		{

		}

		public void NewNote(string uniqueId, string colorStr = "yellow")
		{
			Note note = new Note(uniqueId, colorStr);
			notes.Add(note);
			note.Show();

			NoteAddedEventArgs args = new NoteAddedEventArgs()
			{
				Note = note
			};
			OnNoteAdded(args);

			SaveNoteToDisk(note);
		}

		public void RemoveNote(Note note)
		{
			note.Hide();
			notes.Remove(note);
		}

		public void SaveNoteToDisk(Note note)
		{
			try
			{
				string notesFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tinote", "notes");
				DirectoryInfo di = new DirectoryInfo(notesFolder);

				if (!di.Exists)
				{
					di.Create();
				}

				File.WriteAllText(Path.Combine(notesFolder, note.uniqueId), JsonConvert.SerializeObject(note));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void UpdateNote(NoteData data)
		{
			bool noteExists = false;
			for (int i = 0; i < notes.Count; i++)
			{
				if (notes[i].uniqueId == data.i)
				{
					notes[i].SetTitle(data.l);
					notes[i].SetColor(data.c);
					notes[i].SetContent(data.t);

					noteExists = true;
				}
			}

			if (!noteExists)
			{
				Note note = new Note(data.i, data.c);
				note.SetContent(data.t);
				note.SetTitle(data.l);

				notes.Add(note);

				NoteAddedEventArgs args = new NoteAddedEventArgs()
				{
					Note = note
				};
				OnNoteAdded(args);
			}
		}

		public void LoadNotesFromDisk()
		{
			try
			{
				string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Tinote", "notes");
				DirectoryInfo di = new DirectoryInfo(appDataFolder);

				if (!di.Exists)
				{
					di.Create();
				}

				string[] filePaths = Directory.GetFiles(appDataFolder);

				notes = new List<Note>();

				for (int i = 0; i < filePaths.Length; i++)
				{
					string[] lines = File.ReadAllLines(filePaths[i]);
					Note note = JsonConvert.DeserializeObject<Note>(lines[0]);
					notes.Add(note);

					NoteAddedEventArgs args = new NoteAddedEventArgs()
					{
						Note = note
					};
					OnNoteAdded(args);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		public void LoadNotesFromServer()
		{

		}

		protected virtual void OnNoteAdded(NoteAddedEventArgs e)
		{
			NoteAdded?.Invoke(this, e);
		}
		public event EventHandler<NoteAddedEventArgs> NoteAdded;
	}

	public class NoteAddedEventArgs : EventArgs
	{
		public Note Note { get; set; }
	}
}
