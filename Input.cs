using System.Runtime.CompilerServices;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Profanity;
public class Input
{
    public const ButtonState ButtonUp = ButtonState.Released;
    public const ButtonState ButtonDown = ButtonState.Pressed;
    
    //Keyboard

    public KeyboardState kb, pkb;
    public bool shift_down, ctrl_down, alt_down, shift_press, ctrl_press, alt_press;
    public bool prev_shift_down, prev_ctrl_down, prev_alt_down;
    
    //Mouse
    public MouseState ms, pms;
    public bool leftClick, middleClick, rightClick, leftDown, middleDown, rightDown;
    public int mouseX, mouseY;
    public Vector2 mosPos;
    public Point mp;

    private float screenScaleX, screenScaleY;

    public Input(PresentationParameters pp, RenderTarget2D target)
    {
        screenScaleX = 1.0f / ((float)pp.BackBufferWidth / target.Width);
        screenScaleY = 1.0f / ((float)pp.BackBufferHeight / target.Height);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool KeyPress(Keys k)
    {
        if (kb.IsKeyDown(k) && pkb.IsKeyUp(k)) return true;
        else return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool KeyDown(Keys k)
    {
        if (kb.IsKeyDown(k)) return true;
        else return false;
    }

    public void Update()
    {
        prev_alt_down = alt_down;
        prev_shift_down = shift_down;
        prev_ctrl_down = ctrl_down;
        pkb = kb;
        pms = ms;
        kb = Keyboard.GetState();
        ms = Mouse.GetState();
        
        //Keyboard 
        shift_down = shift_press = ctrl_down = ctrl_press = alt_down = alt_press = false;
        if (kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift)) shift_down = true;
        if (kb.IsKeyDown(Keys.LeftControl) || kb.IsKeyDown(Keys.RightControl)) ctrl_down = true;
        if (kb.IsKeyDown(Keys.LeftAlt) || kb.IsKeyDown(Keys.RightAlt)) alt_down = true;
        if (shift_down && !prev_shift_down) shift_press = true;
        if (ctrl_down && !prev_ctrl_down) ctrl_press = true;
        if (alt_down && !prev_alt_down) alt_press = true;
        
        
        //Mouse
        mosPos = new Vector2(ms.Position.X * screenScaleX, ms.Position.Y * screenScaleY);
        mouseX = (int)mosPos.X;
        mouseY = (int)mosPos.Y;
        mp = new Point(mouseX, mouseY);
        if (ms.LeftButton == ButtonDown) leftDown = true;
        if (ms.MiddleButton == ButtonDown) middleDown = true;
        if (ms.RightButton == ButtonDown) rightDown = true;
        if (leftDown && pms.LeftButton == ButtonUp) leftClick = true;
        if (middleDown && pms.MiddleButton == ButtonUp) middleClick = true;
        if (middleDown && pms.MiddleButton == ButtonUp) middleClick = true;
        if (rightDown && pms.RightButton == ButtonUp) rightClick = true;
        


    }
}