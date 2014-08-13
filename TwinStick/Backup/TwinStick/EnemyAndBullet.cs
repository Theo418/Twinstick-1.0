using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using System.Collections;

namespace TwinStick
{
    public class ParticleObject
    {
        int lifeSpan = 210;
        Vector2 directionVal;
        Vector2 location;
        float rotationVal;
        int transVal = 255;
        Texture2D tex;

        public ParticleObject(Texture2D t)
        {
            tex = t;
        }


        public void setLifeSpan(int life)
        {
            lifeSpan = life;
        }

        public Vector2 direction
        {
            set { directionVal = value; }
            get { return directionVal;}
        }

        public Texture2D texture
        {
            set { tex = value; }
            get { return tex; }
        }

        public int transparency
        {
            set { transVal = value; }
            get { return transVal; }
        }

        public void setLocation(Vector2 loc)
        {
            location = loc;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public int getLifeSpan()
        {
            lifeSpan--;
            return lifeSpan;
        }

        public float rotation
        {
            get { return rotationVal; }
            set { rotationVal = value; }
        }

        public void updateSpeed()
        {
            directionVal.X *= .98F;
            directionVal.Y *= .98F;
            if(transVal > 3)
            transVal -= 2;
        }
    }

    public class Star
    {
        Vector2 starPos;
        int depth;

        public Star(Vector2 sin, int deep)
        {
            starPos = sin;
            depth = deep+2;
        }

        public Vector2 getScrollSpeed()
        {
            return starPos;
        }

        public void setScrollSpeed(Vector2 scrollSpd)
        {
            starPos = scrollSpd;
        }

        public void update(Vector2 ship)
        {
            starPos.X -= ship.X / (10 * depth);
            starPos.Y -= ship.Y / (10 * depth);
        }


    }

    public class Duck
    {
        int timer = 0;
        Vector2 positionL;
        Random r = new Random();
        int moveDistance = 1;
        int dir;
        float rotationVal = 0;


        public Duck(Vector2 pos)
        {
            positionL = pos;
            random();
        }

        public void random()
        {
            dir = r.Next(3);
        }

        public float rotation
        {
            get { return -rotationVal / (float)(180 / Math.PI); }
            set { rotationVal = value; }
        }

        public void rotate()
        {
            if (timer < 60)
            {
                rotationVal += (float)3;
            }
        }

        public Vector2 position
        {
            get { return positionL; }
            set { positionL = value; }
        }

        public void update(float left, float top, float right, float bottom)
        {
            timer++;

            if (timer < 60)
            {

                if (dir == 0)
                    positionL.Y -= moveDistance;
                if (dir == 1)
                    positionL.X += moveDistance;
                if (dir == 2)
                    positionL.Y += moveDistance;
                if (dir == 3)
                    positionL.X -= moveDistance;
            }
            if (timer > 70)
            {
                random();
                if (positionL.X < left + 40)
                    dir = 1;
                if (positionL.X > right - 40)
                    dir = 3;
                if (positionL.Y > bottom - 40)
                    dir = 0;
                if (positionL.Y < top + 40)
                    dir = 2;

                timer = 0;

            }
        }
    }

    public class Bullet
    {
        Vector2 locationVal;
        Vector2 directionVal;
        double angleVal;
        float bulletSpeed;
        Boolean isAlive = true;

        public Bullet(Vector2 loc, Vector2 dir, double ang, float speed)
        {
            locationVal = loc;
            directionVal = dir;
            angleVal = ang;
            bulletSpeed = speed;
        }

        public Vector2 location
        {
            get { return locationVal; }
            set { locationVal = value; }
        }

        public Boolean alive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public double angle
        {
            get { return angleVal; }
            set { angleVal = value; }
        }

        public void update()
        {
            float trajMultiplier = (float)(bulletSpeed / (Math.Pow(directionVal.X, 2) + Math.Pow(directionVal.Y, 2)));
            Vector2 temp;
            temp.X = (float)Math.Sqrt((Math.Pow(directionVal.X, 2) * trajMultiplier));
            temp.Y = (float)Math.Sqrt((Math.Pow(directionVal.Y, 2) * trajMultiplier));
            if (directionVal.X > 0)
                locationVal.X += temp.X;
            if (directionVal.X < 0)
                locationVal.X -= temp.X;
            if (directionVal.Y < 0)
                locationVal.Y += temp.Y;
            if (directionVal.Y > 0)
                locationVal.Y -= temp.Y;

        }
    }

    public class Chaser
    {
        Vector2 positionVal;
        float rotationVal = 0;
        float blockSpeed = 4;


        public Chaser(Vector2 pos)
        {
            positionVal = pos;
        }

        public float rotation
        {
            get { return rotationVal; }
            set { rotationVal = value; }
        }

        public Vector2 position
        {
            get { return positionVal; }
            set { positionVal = value; }
        }

        public void update(Vector2 ship, Vector2 shipSpeed)
        {
            positionVal -= shipSpeed / 2;
            Vector2 tempMovement = new Vector2(positionVal.X - ship.X, positionVal.Y - ship.Y);

            float blockTraj = (float)((Math.Pow(blockSpeed, 2) / (Math.Pow(tempMovement.X, 2) + Math.Pow(tempMovement.Y, 2))));
            if (positionVal.X - ship.X < 0)
                positionVal.X = positionVal.X + (float)Math.Sqrt((Math.Pow(tempMovement.X, 2) * blockTraj));
            if (positionVal.X - ship.X > 0)
                positionVal.X = positionVal.X - (float)Math.Sqrt((Math.Pow(tempMovement.X, 2) * blockTraj));
            if (positionVal.Y - ship.Y < 0)
                positionVal.Y = positionVal.Y + (float)Math.Sqrt((Math.Pow(tempMovement.Y, 2) * blockTraj));
            if (positionVal.Y - ship.Y > 0)
                positionVal.Y = positionVal.Y - (float)Math.Sqrt((Math.Pow(tempMovement.Y, 2) * blockTraj));

            rotationVal -= (float).07;
        }
    }

    public class Rocket
    {
        Vector2 positionVal;
        float rotationVal = 0;
        Boolean vertical;
        int speed = 10;
        Boolean isAlive = true;

        public Rocket(Vector2 pos, Boolean upDown)
        {
            positionVal = pos;
            vertical = upDown;
        }

        public float rotation
        {
            get { return rotationVal; }
            set { rotationVal = value; }
        }

        public Boolean alive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public Vector2 position
        {
            get { return positionVal; }
            set { positionVal = value; }
        }

        public void update(Vector2 shipSpeed, Vector2 leftSide, Vector2 topSide, Vector2 rightSide, Vector2 bottomSide)
        {
            positionVal -= shipSpeed / 2;
            if (vertical)
            {
                if (speed > 0)
                {
                    if (positionVal.Y < bottomSide.Y - 60)
                        positionVal.Y += speed;
                    else
                        speed *= -1;
                }
                else
                {
                    if (positionVal.Y > topSide.Y + 4)
                        positionVal.Y += speed;
                    else
                        speed *= -1;
                }
            }
            else
            {
                if (speed > 0)
                {
                    if (positionVal.X < rightSide.X - 60)
                        positionVal.X += speed;
                    else
                        speed *= -1;
                }
                else
                {
                    if (positionVal.X > leftSide.X + 4)
                        positionVal.X += speed;
                    else
                        speed *= -1;
                }
            }
        }

    }
}