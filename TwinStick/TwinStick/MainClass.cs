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
using BloomPostprocess;

namespace TwinStick {
    public class MainClass : Microsoft.Xna.Framework.Game {
        //Variables
        #region
        int state;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int screenHeight;
        int screenWidth;
        //Border
        Rectangle leftSide;
        Rectangle topSide;
        Rectangle rightSide;
        Rectangle bottomSide;
        Vector2 leftSideLoc;
        Vector2 rightSideLoc;
        Vector2 topSideLoc;
        Vector2 bottomSideLoc;
        //Ship
        Texture2D shipTexture;

        //Input
        GamePadState oldPadState1;
        KeyboardState oldKeyState;
        Vector2 leftStick;
        float leftStickAngle;
        float rightStickAngle;
        int selectorLocation = 0;

        ArrayList bulletList;


        ArrayList enemyList;
        //Sound
        Song myBackgroundMusic;
        SoundEffect bulletSoundEffect;
        SoundEffect deathSFX;
        //Spawn
        int spawnTimer;
        int enemySpawnLevel;

        Boolean screenTransition = false;
        ArrayList particleList;
        ArrayList multiplierList;
        Texture2D chaserEnemyTexture;
        Texture2D snakeEnemyTexture;
        Texture2D spriteTexture;
        Texture2D bulletTexture;
        Texture2D starTexture;
        Texture2D blueBoxTexture;
        Texture2D duckTexture;
        Texture2D worldTexture;
        Texture2D redBoxTexture;
        Texture2D rocketTexture;
        Texture2D introTexture;
        Texture2D whiteBorderTexture;

        Boolean xThreeSixty = true;
        SpriteFont font1;
        Vector2 scorePos;
        int score;
        int lives;
        int multiplierTotal = 0;
        int[] highScores = { 10000, 5000, 2500, 1000, 500 };
        char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        BloomComponent bloom;
        Grid grid;
        Ship ship;
        #endregion

        public MainClass() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 1080; // 544;
            graphics.PreferredBackBufferWidth = 1920; // 960;
            this.graphics.IsFullScreen = false;
            Content.RootDirectory = "Content";
            bloom = new BloomComponent(this);
            Components.Add(bloom);
            bloom.Settings = new BloomSettings(null, 0.25f, 4, 2, 1, 1.5f, 1);
        }

        protected override void Initialize() {
            state = 0;
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            particleList = new ArrayList();
            bulletList = new ArrayList();
            enemyList = new ArrayList();
            multiplierList = new ArrayList();
            //Spawn
            spawnTimer = 400;
            enemySpawnLevel = 5;
            //Game Variables
            score = 0;
            lives = 3;
            ship = new Ship();
            initializeLoc();
            base.Initialize();
        }

        protected override void LoadContent() {
            myBackgroundMusic = Content.Load<Song>("Music2");
            MediaPlayer.IsRepeating = true;
            bulletSoundEffect = Content.Load<SoundEffect>("Laser");
            deathSFX = Content.Load<SoundEffect>("spawn");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteTexture = Content.Load<Texture2D>(@"gfx/Purplebox");
            bulletTexture = Content.Load<Texture2D>(@"gfx/bullet");
            duckTexture = Content.Load<Texture2D>(@"gfx/Duck");
            starTexture = Content.Load<Texture2D>(@"gfx/Star");
            blueBoxTexture = Content.Load<Texture2D>(@"gfx/bluebox");
            chaserEnemyTexture = Content.Load<Texture2D>(@"gfx/Wanderer");
            shipTexture = Content.Load<Texture2D>(@"gfx/Ship3");
            worldTexture = Content.Load<Texture2D>(@"gfx/planet");
            redBoxTexture = Content.Load<Texture2D>(@"gfx/redbox");
            rocketTexture = Content.Load<Texture2D>(@"gfx/titan2");
            introTexture = Content.Load<Texture2D>(@"gfx/titleScreen");
            whiteBorderTexture = Content.Load<Texture2D>(@"gfx/whiteBorder");
            snakeEnemyTexture = Content.Load<Texture2D>(@"gfx/grunt");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font1 = Content.Load<SpriteFont>("SpriteFont1");
            scorePos = new Vector2(screenWidth - 100, 50);
            //Grid
            const int maxGridPoints = 2000;
            Vector2 gridSpacing = new Vector2((float)Math.Sqrt(screenWidth * screenHeight / maxGridPoints));
            grid = new Grid(new Rectangle((int)-80, (int)leftSideLoc.Y - 80, (int)(rightSideLoc.X - leftSideLoc.X), (int)(bottomSideLoc.Y - topSideLoc.Y) + 100), gridSpacing, starTexture);
        }

        protected override void UnloadContent() {

        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            GamePadState padState1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyState = Keyboard.GetState();
            Rectangle enemyRectangle;
            Rectangle bulletRectangle;
            Bullet bullet;
            screenTransition = false;
            #region //Title Menu
            if (state == 0) {
                /*if (padState1.Buttons.A == ButtonState.Pressed) {
                    state = 1;
                    MediaPlayer.Play(myBackgroundMusic);
                }
                if (keyState.IsKeyDown(Keys.T))
                    state = 1;*/
                if (selectorLocation == 0) {
                    if ((keyState.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)) || (padState1.ThumbSticks.Left.Y < -.1 && oldPadState1.ThumbSticks.Left.Y > -.1)) {
                        selectorLocation = 1;
                        bulletSoundEffect.Play();
                    }
                }
                else {
                    if (selectorLocation == 1) {
                        if ((keyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)) || (padState1.ThumbSticks.Left.Y > .1 && oldPadState1.ThumbSticks.Left.Y < .1)) {
                            selectorLocation = 0;
                            bulletSoundEffect.Play();
                        }
                        else {
                            if ((keyState.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)) || (padState1.ThumbSticks.Left.Y < -.1 && oldPadState1.ThumbSticks.Left.Y > -.1)) {
                                selectorLocation = 2;
                                bulletSoundEffect.Play();
                            }
                        }
                    }
                    else
                        if (selectorLocation == 2) {
                            if ((keyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)) || (padState1.ThumbSticks.Left.Y > .1 && oldPadState1.ThumbSticks.Left.Y < .1)) {
                                selectorLocation = 1;
                                bulletSoundEffect.Play();
                            }
                        }
                }
                if ((keyState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)) || (padState1.Buttons.A == ButtonState.Pressed && oldPadState1.Buttons.A == ButtonState.Released)) {
                    if (screenTransition == false) {
                        if (selectorLocation == 0) {
                            state = 1;
                            // MediaPlayer.Play(myBackgroundMusic);}
                            if (selectorLocation == 1)
                                state = 2;
                            if (selectorLocation == 2)
                                state = 3;
                            screenTransition = true;
                            selectorLocation = 0;
                        }
                    }
                }
            }
            #endregion

            if (state == 1 && screenTransition == false) {
                #region //Ship Movement, Angle
                if (padState1.ThumbSticks.Left.X > 0.0f || padState1.ThumbSticks.Left.X < 0.0f ||
                    padState1.ThumbSticks.Left.Y > 0.0f || padState1.ThumbSticks.Left.Y < 0.0f) {
                    leftStick = padState1.ThumbSticks.Left;
                    leftStickAngle = (float)Math.Atan2(-leftStick.Y, leftStick.X);
                }
                if (padState1.ThumbSticks.Left.X > 0.00f)
                    ship.direction.X += padState1.ThumbSticks.Left.X * ship.shipAccelSpeed;
                if (padState1.ThumbSticks.Left.X < -0.00f)
                    ship.direction.X += padState1.ThumbSticks.Left.X * ship.shipAccelSpeed;
                if (padState1.ThumbSticks.Left.Y > 0.00f)
                    ship.direction.Y -= padState1.ThumbSticks.Left.Y * ship.shipAccelSpeed;
                if (padState1.ThumbSticks.Left.Y < -0.00f)
                    ship.direction.Y -= padState1.ThumbSticks.Left.Y * ship.shipAccelSpeed;
                if (keyState.IsKeyDown(Keys.W))
                    ship.direction.Y -= ship.shipAccelSpeed;
                if (keyState.IsKeyDown(Keys.A))
                    ship.direction.X -= ship.shipAccelSpeed;
                if (keyState.IsKeyDown(Keys.S))
                    ship.direction.Y += ship.shipAccelSpeed;
                if (keyState.IsKeyDown(Keys.D))
                    ship.direction.X += ship.shipAccelSpeed;

                float diagSpeed = (ship.direction.X * ship.direction.X) + (ship.direction.Y * ship.direction.Y);
                if (diagSpeed > (ship.maxShipSpeed * ship.maxShipSpeed)) {
                    float xNeg = ship.direction.X;
                    float yNeg = ship.direction.Y;
                    ship.direction.X = (float)Math.Sqrt((ship.direction.X * ship.direction.X * ship.maxShipSpeed * ship.maxShipSpeed) / diagSpeed);
                    ship.direction.Y = (float)Math.Sqrt((ship.direction.Y * ship.direction.Y * ship.maxShipSpeed * ship.maxShipSpeed) / diagSpeed);
                    if (xNeg < 0)
                        ship.direction.X *= -1;
                    if (yNeg < 0)
                        ship.direction.Y *= -1;
                }

                //Border Collision  
                Boolean updateLocX = true;
                Boolean updateLocY = true;
                if (ship.location.X < ship.shipSize + leftSideLoc.X) {
                    if (ship.direction.X < 0)
                        updateLocX = false;
                }
                if (ship.location.X > rightSideLoc.X - ship.shipSize) {
                    if (ship.direction.X > 0)
                        updateLocX = false;
                }
                if (ship.location.Y < topSideLoc.Y + ship.shipSize) {
                    if (ship.direction.Y < 0)
                        updateLocY = false;
                }
                if (ship.location.Y > bottomSideLoc.Y - ship.shipSize) {
                    if (ship.direction.Y > 0)
                        updateLocY = false;
                }
                if (updateLocX) {
                    ship.location.X += (float)(ship.direction.X / 1.5);
                    leftSideLoc.X += -ship.direction.X / 2;
                    rightSideLoc.X += -ship.direction.X / 2;
                    topSideLoc.X += -ship.direction.X / 2;
                    bottomSideLoc.X += -ship.direction.X / 2;
                }
                else
                    ship.direction.X = 0;
                if (updateLocY) {
                    ship.location.Y += (float)(ship.direction.Y / 1.5);
                    leftSideLoc.Y += -ship.direction.Y / 2;
                    rightSideLoc.Y += -ship.direction.Y / 2;
                    topSideLoc.Y += -ship.direction.Y / 2;
                    bottomSideLoc.Y += -ship.direction.Y / 2;
                }
                else
                    ship.direction.Y = 0;

                //Momentum
                ship.direction.X *= (float).935;
                ship.direction.Y *= (float).935;
                #endregion

                #region //Bullet Creation

                rightStickAngle = (float)Math.Atan2(-padState1.ThumbSticks.Right.Y, padState1.ThumbSticks.Right.X);
                ship.rightStickDegrees = -rightStickAngle * (180 / Math.PI);
                /*if(keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Right) ||
                    keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.Up))*/
                if (padState1.ThumbSticks.Right.X > 0.00f || padState1.ThumbSticks.Right.X < -0.00f ||
                    padState1.ThumbSticks.Right.Y > 0.00f || padState1.ThumbSticks.Right.Y < -0.00f) {
                    if (ship.canFire) {
                        Vector2 direction = padState1.ThumbSticks.Right;
                        if (xThreeSixty == false) {
                            int shotX = 0;
                            int shotY = 0;
                            if (keyState.IsKeyDown(Keys.Left))
                                shotX = -1;
                            if (keyState.IsKeyDown(Keys.Right))
                                shotX = 1;
                            if (keyState.IsKeyDown(Keys.Down))
                                shotY = -1;
                            if (keyState.IsKeyDown(Keys.Up))
                                shotY = 1;
                            direction.X = shotX;
                            direction.Y = shotY;
                        }

                        Vector2 bulletLoc;
                        Vector2 direction2;
                        bulletLoc.X = ship.location.X + 20;
                        bulletLoc.Y = ship.location.Y + 20;
                        bulletList.Add(new Bullet(bulletLoc, direction, rightStickAngle, 400));
                        float vLength = (float)Math.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y));
                        if (xThreeSixty) {   //Spread Shot                 
                            double angleUp = rightStickAngle + .04;
                            direction2.X = (float)Math.Cos(angleUp) * vLength;
                            direction2.Y = (float)Math.Sin(angleUp) * -vLength;
                            bulletList.Add(new Bullet(bulletLoc, direction2, angleUp, 300));
                            double angleDown = rightStickAngle - .04;
                            direction2.X = (float)Math.Cos(angleDown) * vLength;
                            direction2.Y = (float)Math.Sin(angleDown) * -vLength;
                            bulletList.Add(new Bullet(bulletLoc, direction2, angleDown, 300));
                        }

                        ship.canFire = false;
                        //bulletSoundEffect.Play();
                    }
                }

                ship.fireTime++;
                if (ship.fireTime > ship.fireFrequency) {
                    ship.canFire = true;
                    ship.fireTime = 0;
                }
                #endregion

                #region //Bullet Collisions
                for (int i = 0; i < bulletList.Count; i++) {
                    bullet = (Bullet)bulletList[i];
                    if (bullet.location.X < leftSideLoc.X || bullet.location.X > rightSideLoc.X ||
                        bullet.location.Y < topSideLoc.Y || bullet.location.Y > bottomSideLoc.Y) {
                        bulletList.RemoveAt(i);
                    }
                    bulletRectangle = new Rectangle((int)bullet.location.X, (int)bullet.location.Y, 40, 40);
                    foreach (Enemy enemy in enemyList) {
                        enemyRectangle = new Rectangle((int)enemy.location.X, (int)enemy.location.Y, 55, 55);
                        if (enemyRectangle.Intersects(bulletRectangle)) {
                            generateEffect(new Vector2(enemy.location.X, enemy.location.Y), blueBoxTexture);
                            bullet.alive = false;
                            enemy.isAlive = false;
                            score += 10 * multiplierTotal;
                            deathSFX.Play();
                            multiplierList.Add(new Multiplier(new Vector2(enemy.location.X, enemy.location.Y)));
                            grid.ApplyExplosiveForce(100, new Vector2(enemy.location.X, enemy.location.Y), 35);
                        }
                    }
                    bullet.update();
                }
                #endregion

                #region //Enemy Spawn
                if (spawnTimer % 200 == 0) {
                    Random random = new Random();
                    int RandX;
                    int RandY;
                    for (int y = 0; y < enemySpawnLevel; y++) {
                        //spawnEffect.Play();
                        RandX = random.Next(screenWidth);
                        RandY = random.Next(screenHeight);
                        if (RandX > ship.location.X + 250 || RandX < ship.location.X - 250 || RandY > ship.location.Y + 250 || RandY < ship.location.Y - 250)
                            if (RandX > leftSideLoc.X + 30 && RandX < rightSideLoc.X - 30 && RandY > topSideLoc.Y + 30 && RandY < bottomSideLoc.Y - 30) {
                                enemyList.Add(new Chaser(new Vector2(RandX, RandY), chaserEnemyTexture));
                            }
                    }
                    for (int y = 0; y < enemySpawnLevel / 10; y++) {
                        RandX = random.Next(screenWidth);
                        RandY = random.Next(screenHeight);
                        if (RandX > ship.location.X + 150 || RandX < ship.location.X - 150 || RandY > ship.location.Y + 150 || RandY < ship.location.Y - 150)
                            if (RandX > leftSideLoc.X + 30 && RandX < rightSideLoc.X - 30 && RandY > topSideLoc.Y + 30 && RandY < bottomSideLoc.Y - 30) {
                                enemyList.Add(new Duck(new Vector2(RandX, RandY), duckTexture));
                            }
                    }

                    for (int y = 0; y < enemySpawnLevel / 10; y++) {
                        RandX = random.Next(screenWidth) + (int)leftSideLoc.X - 50;
                        RandY = 50;
                        int RandB = random.Next(2);
                        Boolean vertical = true;
                        if (RandB == 1) {
                            vertical = false;
                            RandX = 10;
                            RandY = (int)topSideLoc.Y + random.Next(screenHeight) - 50;
                        }
                        if (RandX > ship.location.X + 250 || RandX < ship.location.X - 250 || RandY > ship.location.Y + 250 || RandY < ship.location.Y - 250)
                            if (RandX > leftSideLoc.X + 30 && RandX < rightSideLoc.X - 30 && RandY > topSideLoc.Y + 30 && RandY < bottomSideLoc.Y - 30) {
                                enemyList.Add(new Rocket(new Vector2(RandX, RandY), vertical, rocketTexture));
                            }
                    }
                    for (int y = 0; y < enemySpawnLevel / 10; y++) {
                        RandX = random.Next(screenWidth);
                        RandY = random.Next(screenHeight);
                        if (RandX > ship.location.X + 150 || RandX < ship.location.X - 150 || RandY > ship.location.Y + 150 || RandY < ship.location.Y - 150)
                            if (RandX > leftSideLoc.X + 30 && RandX < rightSideLoc.X - 30 && RandY > topSideLoc.Y + 30 && RandY < bottomSideLoc.Y - 30) {
                                enemyList.Add(new Snake(new Vector2(RandX, RandY), snakeEnemyTexture));
                            }
                    }
                }
                spawnTimer++;
                if (spawnTimer > 700) {
                    enemySpawnLevel += 4;
                    spawnTimer = 0;
                }
                #endregion

                #region //Hit Detection(Death + Multiplier)
                Rectangle shipRectangle = new Rectangle((int)ship.location.X, (int)ship.location.Y, 50, 50);
                foreach (Enemy enemy in enemyList) {
                    enemyRectangle = new Rectangle((int)enemy.location.X, (int)enemy.location.Y, 20, 20);
                    if (shipRectangle.Intersects(enemyRectangle)) {
                        lives--;
                        spawnTimer = 1;
                        clearBlockList();
                        initializeLoc();
                        break;
                    }
                }
                foreach (Multiplier multiplier in multiplierList) {
                    Rectangle multiplyerRectangle = new Rectangle((int)multiplier.getLocation().X, (int)multiplier.getLocation().Y, 20, 20);
                    if (shipRectangle.Intersects(multiplyerRectangle)) {
                        multiplier.alive = false;
                        multiplierTotal++;
                    }
                }
                #endregion

                #region //Location Updates
                foreach (Enemy enemy in enemyList) {
                    enemy.update(ship.location, ship.direction, leftSideLoc.X, topSideLoc.Y, rightSide.X, bottomSide.Y);
                }
                /* foreach (Star s in starList) {
                     s.update(shipSpeed);
                 }*/
                foreach (Multiplier p in multiplierList) {
                    p.update(ship.direction);
                }
                for (int i = 0; i < bulletList.Count; i++) {
                    Bullet b = (Bullet)bulletList[i];
                    if (b.alive == false) {
                        bulletList.RemoveAt(i);
                    }
                }
                for (int i = 0; i < enemyList.Count; i++) {
                    Enemy enemy = (Enemy)enemyList[i];
                    if (enemy.isAlive == false) {
                        enemyList.RemoveAt(i);
                    }
                }
                for (int i = 0; i < multiplierList.Count; i++) {
                    Multiplier m = (Multiplier)multiplierList[i];
                    if (m.alive == false) {
                        multiplierList.RemoveAt(i);
                    }
                }
                updateParticles();
                //Border
                leftSide = new Rectangle((int)leftSideLoc.X, (int)leftSideLoc.Y, 3, screenHeight + 150);
                rightSide = new Rectangle((int)rightSideLoc.X, (int)rightSideLoc.Y, 3, screenHeight + 150);
                topSide = new Rectangle((int)topSideLoc.X, (int)topSideLoc.Y, screenWidth + 20, 3);
                bottomSide = new Rectangle((int)bottomSideLoc.X, (int)bottomSideLoc.Y, screenWidth + 20, 3);

                //Update Grid
                grid.Update();

                #endregion

                #region //State Transition Checks
                if ((keyState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)) || (padState1.Buttons.A == ButtonState.Pressed && oldPadState1.Buttons.A == ButtonState.Released)) {
                    state = 3;
                    screenTransition = true;
                    selectorLocation = 0;
                    MediaPlayer.Pause();
                }
                if (lives == 0) {
                    state = 2;
                    MediaPlayer.Pause();
                    for (int i = 0; i <= 4; i++) {
                        if (score > highScores[i]) {
                            highScores[i] = score;
                            score = 0;
                        }
                    }
                }
                #endregion
            }

            #region //High Score Screen
            if (state == 2 && screenTransition == false) {
                updateParticles();
                if ((keyState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)) || (padState1.Buttons.A == ButtonState.Pressed && oldPadState1.Buttons.A == ButtonState.Released)) {
                    state = 1;
                    lives = 3;
                    score = 0;
                }
                if (padState1.Buttons.B == ButtonState.Pressed && oldPadState1.Buttons.B == ButtonState.Released) {
                    state = 0;
                    lives = 3;
                    score = 0;
                }
            }
            #endregion

            #region //Pause Screen
            if (state == 3 && screenTransition == false) {
                if (selectorLocation == 0) {
                    if ((keyState.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)) || (padState1.ThumbSticks.Left.Y < -.1 && oldPadState1.ThumbSticks.Left.Y > -.1)) {
                        selectorLocation = 1;
                        bulletSoundEffect.Play();
                    }
                }
                else {
                    if (selectorLocation == 1) {
                        if ((keyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)) || (padState1.ThumbSticks.Left.Y > .1 && oldPadState1.ThumbSticks.Left.Y < .1)) {
                            selectorLocation = 0;
                            bulletSoundEffect.Play();
                        }
                        else {
                            if ((keyState.IsKeyDown(Keys.Down) && oldKeyState.IsKeyUp(Keys.Down)) || (padState1.ThumbSticks.Left.Y < -.1 && oldPadState1.ThumbSticks.Left.Y > -.1)) {
                                selectorLocation = 2;
                                bulletSoundEffect.Play();
                            }
                        }
                    }
                    else
                        if (selectorLocation == 2) {
                            if ((keyState.IsKeyDown(Keys.Up) && oldKeyState.IsKeyUp(Keys.Up)) || (padState1.ThumbSticks.Left.Y > .1 && oldPadState1.ThumbSticks.Left.Y < .1)) {
                                selectorLocation = 1;
                                bulletSoundEffect.Play();
                            }
                        }
                }

                if ((keyState.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter)) || (padState1.Buttons.A == ButtonState.Pressed && oldPadState1.Buttons.A == ButtonState.Released)) {
                    if (screenTransition == false) {
                        if (selectorLocation == 0) {
                            state = 1;
                            MediaPlayer.Resume();
                        }
                        if (selectorLocation == 1)
                            state = 2;
                        if (selectorLocation == 2)
                            state = 3;
                        screenTransition = true;
                        selectorLocation = 0;
                    }
                }
            }
            #endregion
            oldPadState1 = padState1;
            oldKeyState = keyState;
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime) {
            bloom.BeginDraw();
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();//SpriteSortMode.Texture, BlendState.Additive

            if (state == 0) {
                //spriteBatch.Draw(introTexture, new Rectangle(0, 0, (int)screenWidth, (int)screenHeight), Color.White);
                spriteBatch.DrawString(font1, "New Game", new Vector2((int)(screenWidth / 2) - 90, (int)(screenHeight * 4 / 7)), new Color(0, 215, 255, 150));
                spriteBatch.DrawString(font1, "Leaderboards", new Vector2((int)(screenWidth / 2) - 90, (int)(screenHeight * 5 / 7)), new Color(0, 215, 255, 150));
                spriteBatch.DrawString(font1, "Exit", new Vector2((int)(screenWidth / 2) - 90, (int)(screenHeight * 6 / 7)), new Color(0, 215, 255, 150));
                spriteBatch.Draw(whiteBorderTexture, new Rectangle((int)((screenWidth * 1 / 2) - (300 / 2)), (int)((screenHeight * ((selectorLocation + 4)) / 7) - (75 / 2)), 300, 75), null, Color.LimeGreen);

            }
            if (state == 1 || state == 3) {
                //Draw Grid
                grid.Draw(spriteBatch, leftSideLoc);
                //Draw Star
                /*foreach (Star s in starList) {
                    //spriteBatch.Draw(starTexture, new Rectangle((int)s.getScrollSpeed().X, (int)s.getScrollSpeed().Y, 3, 3), null, new Color(255, 255, 255, 100), 0, new Vector2(5, 5), SpriteEffects.None, 1);
                }*/
                //spriteBatch.Draw(worldTexture, new Rectangle((int)planetPos.X, (int)planetPos.Y, (int)(340 * .75), (int)(450 * .75)), Color.White);          
                //Draw Block Enemies
                /*foreach (Chaser c in chaserList)
                {
                    spriteBatch.Draw(chaserEnemyTexture, new Rectangle((int)c.location.X, (int)c.location.Y, 55, 55), null, Color.White, c.rotation, new Vector2(50, 50), SpriteEffects.None, 1);
                }

                //Draw Ducks
                foreach (Duck d in duckList) {
                    spriteBatch.Draw(duckTexture, new Rectangle((int)d.location.X, (int)d.location.Y, 55, 55), null, Color.White, d.rotation, new Vector2(50, 50), SpriteEffects.None, 1);
                }*/
                /*
                                //Draw Rockets
                                foreach (Rocket r in rocketList)
                                {
                                    spriteBatch.Draw(rocketTexture, new Rectangle((int)r.position.X, (int)r.position.Y, 45, 45), Color.White);
                                }

                                //Draw Snake
                                foreach (Snake s in snakeList)
                                {
                                    spriteBatch.Draw(snakeEnemyTexture, new Rectangle((int)s.position.X, (int)s.position.Y, 55, 55), null, Color.White, 0, new Vector2(50, 50), SpriteEffects.None, 1);
                                    for (int x = 0; x < s.snakeTails.Length - 1; x++)
                                    {
                                        spriteBatch.Draw(snakeEnemyTexture, new Rectangle((int)s.snakeTails[x].X, (int)s.snakeTails[x].Y, 25, 25), null, Color.White, 0, new Vector2(50, 50), SpriteEffects.None, 1);
                                    }
                                }
                                */
                foreach (Enemy enemy in enemyList) {
                    spriteBatch.Draw(enemy.enemyTexture, new Rectangle((int)enemy.location.X, (int)enemy.location.Y, 50, 50), null, Color.White, enemy.getRotation, new Vector2(50, 50), SpriteEffects.None, 1);
                }
                foreach (Bullet b in bulletList) {
                    spriteBatch.Draw(bulletTexture, new Rectangle((int)b.location.X, (int)b.location.Y, 8, 8), null, Color.White, (float)b.angle, new Vector2(0, 0), SpriteEffects.None, 1);
                    grid.ApplyExplosiveForce(5, b.location, 10);
                }
                foreach (ParticleObject p in particleList) {
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)p.getLocation().X, (int)p.getLocation().Y, 2, 10), null, new Color(255, 255, 255, (byte)p.transparency), p.rotation, new Vector2(5, 5), SpriteEffects.None, 1);
                    //p.texture
                }
                foreach (Multiplier m in multiplierList) {
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)m.getLocation().X, (int)m.getLocation().Y, 12, 12), Color.White);
                }
                spriteBatch.Draw(shipTexture, new Rectangle((int)ship.location.X, (int)ship.location.Y, 55, 55), null, Color.White, leftStickAngle, new Vector2(50, 50), SpriteEffects.None, 1);
                //Score and Lives
                string scoreStr = score.ToString();
                spriteBatch.DrawString(font1, scoreStr, scorePos, new Color(0, 215, 255, 150));
                spriteBatch.DrawString(font1, "Score", new Vector2(scorePos.X, scorePos.Y - 25), new Color(0, 215, 255, 150));
                spriteBatch.DrawString(font1, multiplierTotal.ToString(), new Vector2(scorePos.X, scorePos.Y + 25), Color.White);
                string livesStr = lives.ToString();
                spriteBatch.DrawString(font1, livesStr, new Vector2(25, 50), new Color(0, 215, 255, 150));
                spriteBatch.DrawString(font1, "Lives", new Vector2(25, 25), new Color(0, 215, 255, 150));
                //Draw Border
                spriteBatch.Draw(blueBoxTexture, leftSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, rightSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, topSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, bottomSide, Color.White);
                int outerEdge = 7;
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)leftSideLoc.X - outerEdge, (int)leftSideLoc.Y - outerEdge, 3, screenHeight + 150 + 2 * outerEdge), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)rightSideLoc.X + outerEdge, (int)rightSideLoc.Y - outerEdge, 3, screenHeight + 150 + 2 * outerEdge), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)topSideLoc.X - outerEdge, (int)topSideLoc.Y - outerEdge, screenWidth + 20 + 2 * outerEdge, 3), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)bottomSideLoc.X - outerEdge, (int)bottomSideLoc.Y + outerEdge, screenWidth + 20 + 2 * outerEdge, 3), Color.White);
                if (state == 3) {
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)((screenWidth * 1 / 2) - (300 / 2)), (int)((screenHeight * 7 / 10) - (75 / 2)), 300, 75), null, Color.White);
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)((screenWidth * 1 / 2) - (300 / 2)), (int)((screenHeight * 8 / 10) - (75 / 2)), 300, 75), null, Color.White);
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)((screenWidth * 1 / 2) - (300 / 2)), (int)((screenHeight * 9 / 10) - (75 / 2)), 300, 75), null, Color.White);
                    spriteBatch.Draw(blueBoxTexture, new Rectangle((int)((screenWidth * 1 / 2) - 200), (int)((screenHeight * ((selectorLocation + 7)) / 10) - (20 / 2)), 15, 15), null, Color.White);
                }
            }

            if (state == 2) {
                foreach (ParticleObject p in particleList) {
                    spriteBatch.Draw(p.texture, new Rectangle((int)p.getLocation().X, (int)p.getLocation().Y, 5, 10), null, new Color(255, 255, 255, (byte)p.transparency), p.rotation, new Vector2(5, 5), SpriteEffects.None, 1);
                }
                //Score and Lives
                string scoreStr = score.ToString();
                spriteBatch.DrawString(font1, scoreStr, scorePos, Color.White);
                spriteBatch.DrawString(font1, "Score", new Vector2(scorePos.X, scorePos.Y - 25), Color.White);
                spriteBatch.DrawString(font1, multiplierTotal.ToString(), new Vector2(scorePos.X, scorePos.Y + 25), Color.White);
                string livesStr = lives.ToString();
                spriteBatch.DrawString(font1, livesStr, new Vector2(100, 50), Color.White);
                int y = 75;
                foreach (int i in highScores) {
                    y += 100;
                    string hscoreString = i.ToString();
                    spriteBatch.DrawString(font1, hscoreString, new Vector2(screenWidth / 2 - 170, y), Color.White);
                }
                //High Score
                spriteBatch.DrawString(font1, "High Scores", new Vector2(screenWidth / 2 - 80, 100), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle(screenWidth / 2 - 250, 50, 5, screenHeight - 100), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle(screenWidth / 2 - 250, 50, 505, 5), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle(screenWidth / 2 - 250, screenHeight - 50, 505, 5), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle(screenWidth / 2 + 250, 50, 5, screenHeight - 100), Color.White);
                //Draw Border
                spriteBatch.Draw(blueBoxTexture, leftSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, rightSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, topSide, Color.White);
                spriteBatch.Draw(blueBoxTexture, bottomSide, Color.White);
                int outerEdge = 7;
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)leftSideLoc.X - outerEdge, (int)leftSideLoc.Y - outerEdge, 3, screenHeight + 150 + 2 * outerEdge), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)rightSideLoc.X + outerEdge, (int)rightSideLoc.Y - outerEdge, 3, screenHeight + 150 + 2 * outerEdge), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)topSideLoc.X - outerEdge, (int)topSideLoc.Y - outerEdge, screenWidth + 20 + 2 * outerEdge, 3), Color.White);
                spriteBatch.Draw(blueBoxTexture, new Rectangle((int)bottomSideLoc.X - outerEdge, (int)bottomSideLoc.Y + outerEdge, screenWidth + 20 + 2 * outerEdge, 3), Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected void clearBlockList() {
            //Generate Death Particles
            foreach (Enemy enemy in enemyList) {
                generateEffect(new Vector2(enemy.location.X, enemy.location.Y), spriteTexture);
            }
            //Kill All Enemies
            enemyList.Clear();
            /*int temp = enemyList.Count;
            for (int f = 0; f < temp-1; f++) {
                enemyList.RemoveAt(0);
            }*/
        }

        protected void initializeLoc() {
            //Borders
            leftSideLoc.X = -75;
            leftSideLoc.Y = -150;
            rightSideLoc.X = screenWidth + 75;
            rightSideLoc.Y = -150;
            topSideLoc.X = -75;
            topSideLoc.Y = -150;
            bottomSideLoc.X = -75;
            bottomSideLoc.Y = screenHeight + 150;
            //Clean this up
            leftSide = new Rectangle((int)leftSideLoc.X, (int)leftSideLoc.Y, 3, screenHeight + 300);
            rightSide = new Rectangle((int)rightSideLoc.X, (int)rightSideLoc.Y, 3, screenHeight + 300);
            topSide = new Rectangle((int)topSideLoc.X, (int)topSideLoc.Y, screenWidth + 150, 3);
            bottomSide = new Rectangle((int)bottomSideLoc.X, (int)bottomSideLoc.Y, screenWidth + 150, 3);
            ship.location.X = screenWidth / 2;
            ship.location.Y = screenHeight / 2;
            ship.direction.X = 0;
            ship.direction.Y = 0;
        }

        protected void generateEffect(Vector2 loc, Texture2D tex) {
            Random r = new Random();
            int numOfP = r.Next(160) + 40;
            for (int j = 0; j < numOfP; j++) {
                ParticleObject particle;
                particle = new ParticleObject(tex);
                particle.setLocation(new Vector2(loc.X, loc.Y));
                int x = r.Next(78);
                double realX = (float)((x * .12));
                int y = r.Next(78);
                double realY = (float)((y * .12));
                double maxDist = 90;// (.01 * r.Next(400)) + 40;
                Boolean create = false;
                if (realX * realX + realY * realY < maxDist) {
                    create = true;
                }
                int isNeg = r.Next(2);
                if (isNeg == 0)
                    realX *= -1;
                int isNeg2 = r.Next(2);
                if (isNeg2 == 0)
                    realY *= -1;
                particle.direction = (new Vector2((float)realX * 2, (float)realY * 2));
                particle.rotation = (float)Math.Atan2(realY, realX) - (float)(Math.PI / 2);
                if (create)
                    particleList.Add(particle);
            }
        }

        protected void updateParticles() {
            for (int i = 0; i < particleList.Count; i++) {
                ParticleObject p = (ParticleObject)particleList[i];
                float x = p.getLocation().X + p.direction.X - ship.direction.X / 2;
                float y = p.getLocation().Y + p.direction.Y - ship.direction.Y / 2;
                /*if (x < leftSideLoc.X)
                {
                    p.setDirection(new Vector2(p.getDirection().X * -1, p.getDirection().Y));
                    x = leftSideLoc.X + 3;
                }
                if (x > rightSideLoc.X)
                {
                    p.setDirection(new Vector2(p.getDirection().X * -1, p.getDirection().Y));
                    x = rightSideLoc.X - 1;
                }
                if (y < topSideLoc.Y)
                {
                    p.setDirection(new Vector2(p.getDirection().X, p.getDirection().Y * -1));
                    y = topSideLoc.Y + 3;
                }
                if (y > bottomSideLoc.Y)
                {
                    p.setDirection(new Vector2(p.getDirection().X, p.getDirection().Y * -1));
                    y = bottomSideLoc.Y - 1;
                }*/
                p.updateSpeed();
                p.setLocation(new Vector2(x, y));
                int life = p.getLifeSpan();
                if (p.getLifeSpan() < 0) {
                    particleList.RemoveAt(i);
                }

            }
        }
    }

}

