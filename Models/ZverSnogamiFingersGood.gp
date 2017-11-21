double a=1; 
double b=1; 
double c=1;
if (!secrettingNow[3] && !secrettingNow[4] && cellType<11) cellType=cellType;
if (cellType==0)
{
if (sensorReaction[2]==0)
{	resilience=0.4;
	if (numDivisions<8)
	{	SpawnPolarization(true,1);	}
	else
	{   if (neighbourCount<(21 + rnd*1.05))
	    {	if (sensorReaction[0]<0.0001)
			{	Spill(0); 	}  //Anterior organizer
			else
			{
			if (!secrettingNow[0] && sensorReaction[1]<0.0001)
				{	Spill(1);	}	// Posterior organizer	
			}
		}		
		if (secrettingNow[1])  // Tail growth
		{
			if (numDivisions<11)
			{	SpawnGradient(0,true,1).numDivisions=4;
			}		
			if (numDivisions>10)
			{
				SpawnGradient(0,true,0);
			}
			if (sensorReaction[0]<0.04)
			{
				Spill(2);
			}
		}
	}
}

//CELL DIFFERENTIATION STARTS!
if (sensorReaction[2]>0)  
{
a=sensorReaction[0]/sensorReaction[1];
if (a<3.05 && a>2.95 && sensorReaction[3]<0.00001 & EnvironmentalAccess>0.05)
{
	Spill(3); cellType=10; //Dorsal organizer	
}
b=sensorReaction[3];
if (a<3.2 && a>(2.4 - rnd*0.2) && EnvironmentalAccess>0.03 && b>0 && (b<0.085 || (b<0.1 && rnd<0.05)) && sensorReaction[4]<0.00001)
{
	Spill(4); cellType=11;  // Ventral organizer	
}	
c=sensorReaction[3]-sensorReaction[4];
if (!secrettingNow[0] && !secrettingNow[1] && b>0 && sensorReaction[4]>0)
		{
			if (EnvironmentalAccess>0)
			{	
				if (c>0) 
			    {
					cellType=1; //dorsal ectoderm
				}
				else
				{
					cellType=2; //ventral ectoderm
				}					
			}
			else
			{	
				if (c>0)
				{
					cellType=3; //dorsal endoderm				
				}
				else
				{
					cellType=4; //ventral ectoderm
				}
			}
		}
}
}

//ECTODERM & ENDODERM BEHAVIOUR
if (cellType==1) //dorsal ectoderm
{	radius=1.1;
	c=sensorReaction[3]-sensorReaction[4];
	if (c<0) cellType=2;
	
	// EYES
	if (sensorReaction[0]>0.18 && sensorReaction[0]<0.205 && c<0.045 && c>0.015 && sensorReaction[6]<0.045)
	{
		radius=1.3; Spill(6); cellType=6; numDivisions=0;
		if (EnvironmentalAccess<0.1) MoveFromTheCrowd(false,0.5);
	}	
}
if (cellType==2) //ventral ectoderm
{   
	c=sensorReaction[3]-sensorReaction[4]; // Dorsoventral coordinate
	if (c>0) cellType=1;
	
	//FEET
	a=sensorReaction[0]; // From anterior
	b=sensorReaction[7]; // From other feet
	 
	//if (((c>-0.026) || (rnd<0.02 && c>-0.05)) && ((sensorReaction[7]<0.03)||(rnd<0.05 && sensorReaction[7]<0.07)) && ((a>1 && a<1.6)||(a>1 && a<3.35)))
	if (((a<0.19 && a>0.16) || (a<0.115 && a>0.091)) && (b<0.03||(rnd<0.05 && b<0.06)) && (c>-0.025 || (rnd<0.03 && c>-0.035)))
	{
		Spill(7);
		cellType=7;
		numDivisions=0;
	}
	    
}
if (cellType==3) //dorsal endoderm
{	radius=0.8;
    c=sensorReaction[3]-sensorReaction[4];
	if (c<0) cellType=4;
}
if (cellType==4) //ventral endoderm
{   radius=0.7;
    c=sensorReaction[3]-sensorReaction[4];
	if (c>0) cellType=3;
}
if (cellType==3 || cellType==4)
{
  if (EnvironmentalAccess>0)  cellType=0;
}
if (cellType==6) // Eyes
{
   if (numDivisions<5)
   {
	   radius = radius/1.18;
	   SpawnWherever();
   }
}
if (cellType==7) // Feet progenitors
{
	if (numDivisions<1)
	{
		SpawnGradient(3,false,0).cellType=8;
	}
}
if (cellType==8) // Feet
{
	Spill(8);
	if (numDivisions<8)
	{
		MoveGradient(3,false,false,0.5);
		SpawnGradient(3,true,0).cellType=9;
	}
	else
	{
		if(numDivisions<13)
		{
			MoveGradient(1,false,false,0.5);
			SpawnGradient(1,true,0).cellType=9;
		}
		else
		{
		if(numDivisions<16)
		{
			polarization = gradient[1] + gradient[4];
			SpawnPolarization(false,20).cellType=10;
		}
		}
	}
}
if (cellType==9)
{radius=0.8;}
if (cellType==10) // Fingers
{
	radius=0.5;
	if(numDivisions<21)
	{
		MoveGradient(8,false,false,0.5);
	    SpawnGradient(8,true,0).cellType=11;
	}
}
if (cellType==11)
{
	cellType=1;
}



