using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleFigures {
  public class InputManager {
    public MouseState MouseStNow;
    public MouseState MouseStPrev;
    public KeyboardState KeyboardStNow;
    public KeyboardState KeyboardStPrev;

    public delegate void MouseEventDelegate(MouseState msn, MouseState msp);
    public event MouseEventDelegate MouseEvent;
    public delegate void KeyboardEventDelegate(KeyboardState ksn, KeyboardState ksp);
    public event KeyboardEventDelegate KeyboardEvent;

    public Thread MouseThread;
    public Thread KeyboardThread;

    protected bool listenMouse = true, listenKeyboard = true;

    public bool ListenMouse {
      get { return this.listenMouse; }
      set { if (!this.MouseThread.IsAlive) this.listenMouse = value; }
    }
    public bool ListenKeyboard {
      get { return this.listenKeyboard; }
      set { if (!this.KeyboardThread.IsAlive) this.listenKeyboard = value; }
    }

    public InputManager() {
      this.MouseThread = new Thread(this.DoListenMouse);
      this.KeyboardThread = new Thread(this.DoListenKeyboard);
      this.MouseThread.IsBackground = true;
      this.KeyboardThread.IsBackground = true;
    }

    public InputManager(bool listenMouse, bool listenKeyboard) {
      this.MouseThread = new Thread(this.DoListenMouse);
      this.KeyboardThread = new Thread(this.DoListenKeyboard);
      this.MouseThread.IsBackground = true;
      this.KeyboardThread.IsBackground = true;
      this.listenMouse = listenMouse;
      this.listenKeyboard = listenKeyboard;
    }

    public void StartListenInput() {
      if (this.listenMouse) this.MouseThread.Start();
      if (this.listenKeyboard) this.KeyboardThread.Start();
    }

    public void StopListenInput() {
      if (this.listenMouse) this.MouseThread.Abort();
      if (this.listenKeyboard) this.KeyboardThread.Abort();
    }

    void DoListenMouse() {
      while (true) {
        this.MouseStNow = Mouse.GetState();

        if (this.MouseStNow != this.MouseStPrev) {
          if (this.MouseEvent != null) this.MouseEvent(this.MouseStNow, this.MouseStPrev);
        }

        this.MouseStPrev = this.MouseStNow;
        Thread.Sleep(1);
      }
    }

    void DoListenKeyboard() {
      while (true) {
        this.KeyboardStNow = Keyboard.GetState();

        if (this.KeyboardStNow != this.KeyboardStPrev) {
          if (this.KeyboardEvent != null) this.KeyboardEvent(this.KeyboardStNow, this.KeyboardStPrev);
        }

        this.KeyboardStPrev = this.KeyboardStNow;
        Thread.Sleep(1);
      }
    }
  }

  public class MouseEventArgs : EventArgs {
    public MouseState MouseStNow;
    public MouseState MouseStPrev;

    public MouseEventArgs(MouseState msn, MouseState msp) {
      this.MouseStNow = msn;
      this.MouseStPrev = msp;
    }
  }

  public class KeyboardEventArgs : EventArgs {
    public KeyboardState KeyboardStNow;
    public KeyboardState KeyboardStPrev;

    public KeyboardEventArgs(KeyboardState ksn, KeyboardState ksp) {
      this.KeyboardStNow = ksn;
      this.KeyboardStPrev = ksp;
    }
  }
}
