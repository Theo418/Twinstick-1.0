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
using System.Collections;

namespace TwinStick {

    public class Ship {
        public Vector2 location;
        public Vector2 direction;
        public float rotation;
        public Boolean isAlive;
        public Texture2D enemyTexture;
        public int fireTime;
        public int fireFrequency;
        public Boolean canFire;
        public float shipAccelSpeed;
        public int shipSize;
        public int maxShipSpeed;
        public double rightStickDegrees;

        public Ship() {
            shipAccelSpeed = 1.2f;
            shipSize = 25;
            maxShipSpeed = 8;
            fireTime = 10;
            fireFrequency = 6;
            canFire = true;
            location = new Vector2(0, 0);
            direction = new Vector2(0, 0);
        }
        public float getRotation {
            get { return -rotation / (float)(180 / Math.PI); }
            set { rotation = value; }
        }
        public Vector2 getLocation {
            get { return location; }
            set { location = value; }
        }
        public virtual void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {

        }
    }
    public class Enemy {
        public Vector2 location;
        public Vector2 direction;
        public float speed;
        public float rotation;
        public Boolean isAlive;
        public int timer;
        public Texture2D enemyTexture;

        public Enemy() {

        }
        public float getRotation {
            get { return -rotation / (float)(180 / Math.PI); }
            set { rotation = value; }
        }
        public Vector2 getLocation {
            get { return location; }
            set { location = value; }
        }
        public virtual void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {

        }
    }

    public class Duck : Enemy {
        Random r = new Random();
        int fourWayDirection;

        public Duck(Vector2 pos,Texture2D tex) {
            location = pos;
            random();
            timer = 0;
            speed = 1;
            isAlive = true;
            enemyTexture = tex;  
        }
        public void random() {
            fourWayDirection = r.Next(3);
        }

        public override void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {
            if (timer < 60) 
                rotation += 3;
            location.X = location.X - shipSpeed.X / 2;  
            location.Y = location.Y - shipSpeed.Y / 2;
            timer++;
            if (timer < 60) {
                if (fourWayDirection == 0)
                    location.Y -= speed;
                if (fourWayDirection == 1)
                    location.X += speed;
                if (fourWayDirection == 2)
                    location.Y += speed;
                if (fourWayDirection == 3)
                    location.X -= speed;
            }
            if (timer > 70) {
                random();
                if (location.X < left + 40)
                    fourWayDirection = 1;
                if (location.X > right - 40)
                    fourWayDirection = 3;
                if (location.Y > bottom - 40)
                    fourWayDirection = 0;
                if (location.Y < top + 40)
                    fourWayDirection = 2;
                timer = 0;
            }
        }
    }

    public class Chaser : Enemy {
        public Chaser(Vector2 pos, Texture2D tex) {
            location = pos;
            speed = 4;
            rotation = 0;
            isAlive = true;
            enemyTexture = tex;
        }
        public override void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {
            location -= shipSpeed / 2;
            rotation += 3;
            Vector2 tempMovement = new Vector2(location.X - ship.X, location.Y - ship.Y);
            float blockTraj = (float)((Math.Pow(speed, 2) / (Math.Pow(tempMovement.X, 2) + Math.Pow(tempMovement.Y, 2))));
            if (location.X - ship.X < 0)
                location.X = location.X + (float)Math.Sqrt((Math.Pow(tempMovement.X, 2) * blockTraj));
            if (location.X - ship.X > 0)
                location.X = location.X - (float)Math.Sqrt((Math.Pow(tempMovement.X, 2) * blockTraj));
            if (location.Y - ship.Y < 0)
                location.Y = location.Y + (float)Math.Sqrt((Math.Pow(tempMovement.Y, 2) * blockTraj));
            if (location.Y - ship.Y > 0)
                location.Y = location.Y - (float)Math.Sqrt((Math.Pow(tempMovement.Y, 2) * blockTraj));
            rotation -= (float).07;
        }
    }

    public class Rocket : Enemy {
        Boolean vertical;
        public Rocket(Vector2 pos, Boolean upDown, Texture2D tex) {
            location = pos;
            vertical = upDown;
            speed = 10;
            isAlive = true;
            enemyTexture = tex;
        }
        public override void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {
            location -= shipSpeed / 2;
            if (vertical) {
                if (speed > 0) {
                    if (location.Y < bottom - 60)
                        location.Y += speed;
                    else
                        speed *= -1;
                }
                else {
                    if (location.Y > top + 4)
                        location.Y += speed;
                    else
                        speed *= -1;
                }
            }
            else {
                if (speed > 0) {
                    if (location.X < right - 60)
                        location.X += speed;
                    else
                        speed *= -1;
                }
                else {
                    if (location.X > left + 4)
                        location.X += speed;
                    else
                        speed *= -1;
                }
            }
        }
    }
    
    public class Snake : Enemy {
        Random r = new Random();
        int direction;
        Vector2[] snakeTail = new Vector2[7];
        int[] snakeTailDirection = new int[7];

        public Snake(Vector2 pos, Texture2D tex) {
            location = pos;
            for (int x = 0; x < snakeTail.Length; x++) {
                snakeTail[x] = pos;
            }
            for (int x = 0; x < snakeTailDirection.Length; x++) {
                snakeTailDirection[x] = -1;
            }
            random();
            speed = 2;
            isAlive = true;
            enemyTexture = tex;
        }
        public void random() {
            direction = r.Next(3);
        }
        public Vector2[] snakeTails {
            get { return snakeTail; }
            set { snakeTail = value; }
        }
        public override void update(Vector2 ship, Vector2 shipSpeed, float left, float top, float right, float bottom) {
            location.X -= shipSpeed.X / 2;
            location.Y -= shipSpeed.Y / 2;
            timer++;
            if (timer <= 60) {
                if (direction == 0) location.Y -= speed;
                if (direction == 1) location.X += speed;
                if (direction == 2) location.Y += speed;
                if (direction == 3) location.X -= speed;
                for (int x = 0; x < snakeTail.Length; x++) {
                    if (snakeTailDirection[x] == 0)
                        snakeTail[x].Y -= speed;
                    if (snakeTailDirection[x] == 1)
                        snakeTail[x].X += speed;
                    if (snakeTailDirection[x] == 2)
                        snakeTail[x].Y += speed;
                    if (snakeTailDirection[x] == 3)
                        snakeTail[x].X -= speed;
                }
            }
            if (timer == 15 || timer == 30 || timer == 45) {
                snakeTailDirection[0] = direction;
                for (int x = snakeTail.Length - 1; x >= 1; x--) {
                    snakeTailDirection[x] = snakeTailDirection[x - 1];
                }
            }
            if (timer == 60) {
                for (int x = snakeTail.Length - 1; x >= 1; x--) {
                    snakeTailDirection[x] = snakeTailDirection[x - 1];
                }
                snakeTailDirection[0] = direction;
                random();
                if (location.X < left + 40)
                    direction = 1;
                if (location.X > right - 40)
                    direction = 3;
                if (location.Y > bottom - 40)
                    direction = 0;
                if (location.Y < top + 40)
                    direction = 2;
                timer = 0;
            }       
            for (int i = 0; i < snakeTails.Length - 1; i++) {
                snakeTails[i] = new Vector2(snakeTails[i].X - shipSpeed.X / 2,  snakeTails[i].Y - shipSpeed.Y / 2);
            }
        }
    }
  
    public class Bullet {
        Vector2 locationVal;
        Vector2 directionVal;
        double angleVal;
        public float bulletSpeed;
        Boolean isAlive = true;

        public Bullet(Vector2 loc, Vector2 dir, double ang, float speed) {
            locationVal = loc;
            directionVal = dir;
            angleVal = ang;
            bulletSpeed = speed;
        }

        public Vector2 location {
            get { return locationVal; }
            set { locationVal = value; }
        }

        public Boolean alive {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public double angle {
            get { return angleVal; }
            set { angleVal = value; }
        }

        public void update() {
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

    public class ParticleObject {
        int lifeSpan = 310;
        Vector2 directionVal;
        Vector2 location;
        float rotationVal;
        byte transVal = 255;
        Texture2D tex;

        public ParticleObject(Texture2D t) {
            tex = t;
        }


        public void setLifeSpan(int life) {
            lifeSpan = life;
        }

        public Vector2 direction {
            set { directionVal = value; }
            get { return directionVal; }
        }

        public Texture2D texture {
            set { tex = value; }
            get { return tex; }
        }

        public byte transparency {
            set { transVal = value; }
            get { return transVal; }
        }

        public void setLocation(Vector2 loc) {
            location = loc;
        }

        public Vector2 getLocation() {
            return location;
        }

        public int getLifeSpan() {
            lifeSpan--;
            return lifeSpan;
        }

        public float rotation {
            get { return rotationVal; }
            set { rotationVal = value; }
        }

        public void updateSpeed() {
            directionVal.X *= .98F;
            directionVal.Y *= .98F;
            transVal = 10;
        }
    }

    public class Multiplier {
        int lifeSpan = 310;
        Vector2 location;
        Boolean isAlive = true;

        public Multiplier(Vector2 pos) {
            location = pos;
        }

        public void setLifeSpan(int life) {
            lifeSpan = life;
        }

        public void setLocation(Vector2 loc) {
            location = loc;
        }

        public Vector2 getLocation() {
            return location;
        }
        public Boolean alive {
            get { return isAlive; }
            set { isAlive = value; }
        }

        public int getLifeSpan() {
            lifeSpan--;
            return lifeSpan;
        }


        public void update(Vector2 ship) {
            lifeSpan--;
            if (lifeSpan == 0)
                isAlive = false;
            location.X = location.X - ship.X / 2;
            location.Y = location.Y - ship.Y / 2;
        }

    }

    public class Star {
        Vector2 starPos;
        int depth;

        public Star(Vector2 sin, int deep) {
            starPos = sin;
            depth = deep + 2;
        }

        public Vector2 getScrollSpeed() {
            return starPos;
        }

        public void setScrollSpeed(Vector2 scrollSpd) {
            starPos = scrollSpd;
        }

        public void update(Vector2 ship) {
            starPos.X -= ship.X / (10 * depth);
            starPos.Y -= ship.Y / (10 * depth);
        }


    }

    public class Grid
	{
		class PointMass
		{
			public Vector3 Position;
			public Vector3 Velocity;
			public float InverseMass;

			private Vector3 acceleration;
			private float damping = 0.98f;

			public PointMass(Vector3 position, float invMass)
			{
				Position = position;
				InverseMass = invMass;
			}

			public void ApplyForce(Vector3 force)
			{
				acceleration += force * InverseMass;
			}

			public void IncreaseDamping(float factor)
			{
				damping *= factor;
			}

			public void Update()
			{
				Velocity += acceleration;
				Position += Velocity;
				acceleration = Vector3.Zero;
				if (Velocity.LengthSquared() < 0.001f * 0.001f)
					Velocity = Vector3.Zero;

				Velocity *= damping;
				damping = 0.98f;
			}
		}

		struct Spring
		{
			public PointMass End1;
			public PointMass End2;
			public float TargetLength;
			public float Stiffness;
			public float Damping;

			public Spring(PointMass end1, PointMass end2, float stiffness, float damping)
			{
				End1 = end1;
				End2 = end2;
				Stiffness = stiffness;
				Damping = damping;
				TargetLength = Vector3.Distance(end1.Position, end2.Position) * 0.95f;
			}

			public void Update()
			{
				var x = End1.Position - End2.Position;

				float length = x.Length();
				// these springs can only pull, not push
				if (length <= TargetLength)
					return;

				x = (x / length) * (length - TargetLength);
				var dv = End2.Velocity - End1.Velocity;
				var force = Stiffness * x - dv * Damping;

				End1.ApplyForce(-force);
				End2.ApplyForce(force);
			}
		}

		Spring[] springs;
		PointMass[,] points;
		Vector2 screenSize;
        Texture2D texture;
		public Grid(Rectangle size, Vector2 spacing,Texture2D tex)
		{
			var springList  = new List<Spring>();
            texture = tex;
			int numColumns = (int)(size.Width / spacing.X) + 1;
			int numRows = (int)(size.Height / spacing.Y) + 1;
			points = new PointMass[numColumns, numRows];

			// these fixed points will be used to anchor the grid to fixed positions on the screen
			PointMass[,] fixedPoints = new PointMass[numColumns, numRows];

			// create the point masses
			int column = 0, row = 0;
			for (float y = size.Top; y <= size.Bottom; y += spacing.Y)
			{
				for (float x = size.Left; x <= size.Right; x += spacing.X)
				{
					points[column, row] = new PointMass(new Vector3(x, y, 0), 1);
					fixedPoints[column, row] = new PointMass(new Vector3(x, y, 0), 0);
					column++;
				}
				row++;
				column = 0;
			}

			// link the point masses with springs
			for (int y = 0; y < numRows; y++)
				for (int x = 0; x < numColumns; x++)
				{
					if (x == 0 || y == 0 || x == numColumns - 1 || y == numRows - 1)	// anchor the border of the grid
						springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.1f, 0.1f));
					else if (x % 3 == 0 && y % 3 == 0)									// loosely anchor 1/9th of the point masses
						springList.Add(new Spring(fixedPoints[x, y], points[x, y], 0.002f, 0.02f));

					const float stiffness = 0.28f;
					const float damping = 0.06f;

					if (x > 0)
						springList.Add(new Spring(points[x - 1, y], points[x, y], stiffness, damping));
					if (y > 0)
						springList.Add(new Spring(points[x, y - 1], points[x, y], stiffness, damping));
				}

			springs = springList.ToArray();
		}

		public void ApplyDirectedForce(Vector2 force, Vector2 position, float radius)
		{
			ApplyDirectedForce(new Vector3(force, 0), new Vector3(position, 0), radius);
		}

		public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
		{
			foreach (var mass in points)
				if (Vector3.DistanceSquared(position, mass.Position) < radius * radius)
					mass.ApplyForce(10 * force / (10 + Vector3.Distance(position, mass.Position)));
		}

		public void ApplyImplosiveForce(float force, Vector2 position, float radius)
		{
			ApplyImplosiveForce(force, new Vector3(position, 0), radius);
		}

		public void ApplyImplosiveForce(float force, Vector3 position, float radius)
		{
			foreach (var mass in points)
			{
				float dist2 = Vector3.DistanceSquared(position, mass.Position);
				if (dist2 < radius * radius)
				{
					mass.ApplyForce(10 * force * (position - mass.Position) / (100 + dist2));
					mass.IncreaseDamping(0.6f);
				}
			}
		}

		public void ApplyExplosiveForce(float force, Vector2 position, float radius)
		{
			ApplyExplosiveForce(force, new Vector3(position, 0), radius);
		}

		public void ApplyExplosiveForce(float force, Vector3 position, float radius)
		{
			foreach (var mass in points)
			{
				float dist2 = Vector3.DistanceSquared(position, mass.Position);
				if (dist2 < radius * radius)
				{
					mass.ApplyForce(100 * force * (mass.Position - position) / (10000 + dist2));
					mass.IncreaseDamping(0.6f);
				}
			}
		}

		public void Update()
		{
			foreach (var spring in springs)
				spring.Update();

			foreach (var mass in points)
				mass.Update();
		}

		public void Draw(SpriteBatch spriteBatch,Vector2 leftWall)
		{
            screenSize = new Vector2(960, 544);

			int width = points.GetLength(0);
			int height = points.GetLength(1);
			Color color = new Color(30, 30, 139, 85);	// dark blue

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Vector2 left = new Vector2(), up = new Vector2();
					Vector2 p = ToVec2(points[x, y].Position);
                    float thickness = 1;
					if (x > 1)
					{
						left = ToVec2(points[x - 1, y].Position);
						//float thickness = y % 3 == 1 ? 3f : 1f;
						
						// use Catmull-Rom interpolation to help smooth bends in the grid
						int clampedX = Math.Min(x + 1, width - 1);
						Vector2 mid = Vector2.CatmullRom(ToVec2(points[x - 2, y].Position), left, p, ToVec2(points[clampedX, y].Position), 0.5f);
						// If the grid is very straight here, draw a single straight line. Otherwise, draw lines to our
						// new interpolated midpoint
                        if (Vector2.DistanceSquared(mid, (left + p) / 2) > 1)
						{
                            spriteBatch.DrawLine(left, mid, color, texture, leftWall, thickness);
                            spriteBatch.DrawLine(mid, p, color, texture, leftWall, thickness);
						}
						else
                            spriteBatch.DrawLine(left, p, color, texture, leftWall, thickness);
					}
					if (y > 1)
					{
						up = ToVec2(points[x, y - 1].Position);
						//float thickness = x % 3 == 1 ? 3f : 1f;
						int clampedY = Math.Min(y + 1, height - 1);
						Vector2 mid = Vector2.CatmullRom(ToVec2(points[x, y - 2].Position), up, p, ToVec2(points[x, clampedY].Position), 0.5f);
						if (Vector2.DistanceSquared(mid, (up + p) / 2) > 1)
						{
                            spriteBatch.DrawLine(up, mid, color, texture,leftWall, thickness);
                            spriteBatch.DrawLine(mid, p, color, texture,leftWall, thickness);
						}
						else
                            spriteBatch.DrawLine(up, p, color, texture,leftWall, thickness);
					}

					// Add interpolated lines halfway between our point masses. This makes the grid look
					// denser without the cost of simulating more springs and point masses.
					/*if (x > 1 && y > 1)
					{
						Vector2 upLeft = ToVec2(points[x - 1, y - 1].Position);
                        spriteBatch.DrawLine(0.5f * (upLeft + up), 0.5f * (left + p), color, texture,leftWall, 1f);	// vertical line
                        spriteBatch.DrawLine(0.5f * (upLeft + left), 0.5f * (up + p), color, texture,leftWall, 1f);	// horizontal line
					}*/
				}
			}
		}

		public Vector2 ToVec2(Vector3 v)
		{
			// do a perspective projection
			float factor = (v.Z + 2000) / 2000;
			return (new Vector2(v.X, v.Y) - screenSize / 2f) * factor + screenSize / 2;
		}

        
	}
   
    static class Extensions {
        public static float ToAngle(this Vector2 vector) {
            return (float)Math.Atan2(vector.Y, vector.X);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, Texture2D tex, Vector2 leftWall, float thickness = 2f) {
            Vector2 delta = end - start;
            spriteBatch.Draw(tex, start + leftWall, null, (color * .08f), delta.ToAngle(), new Vector2(0, 0.5f), new Vector2(delta.Length(), thickness), SpriteEffects.None, 0f);

        }

    }

  

}