double c=1;
if (cellType==0)
{
if (sensorReaction[2]==0)
{	resilience=0.4;
	if (numDivisions<8)
	{	SpawnPolarization(true,1);	}
	else
	{   if (neighbourCount<21)
	    {	if (sensorReaction[0]<0.0001)
			{	cellType=8; Spill(0); 	}  //Anterior organizer
			else
			{
			if (!secrettingNow[0] && sensorReaction[1]<0.0001)
				{	cellType=2; Spill(1);	}	// Posterior organizer	
			}
		}		
		if (secrettingNow[1])  // Tail growth
		{
			if (numDivisions<11)
			{	SpawnGradient(0,true,1).numDivisions=4;
			}		
			if (numDivisions>10)
			{
				cellType=3;
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
double a=sensorReaction[0]/sensorReaction[1];
if (a<3.05 && a>2.95 && sensorReaction[3]<0.00001 & EnvironmentalAccess>0.05)
{
	Spill(3);  //Dorsal organizer
	cellType=3;
}
double b=1;
b=sensorReaction[3];
if (a<3.3 && a>2.4 && EnvironmentalAccess>0.03 && b>0 && b<0.085 && sensorReaction[4]<0.00001)
{
	Spill(4);  // Ventral organizer
	cellType=5;
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
	if (!secrettingNow[3]) cellType=4;
	c=sensorReaction[3]-sensorReaction[4];
	if (c<0) cellType=2;
}
if (cellType==2) //ventral ectoderm
{   
	if (!secrettingNow[4]) cellType=6;
	c=sensorReaction[3]-sensorReaction[4];
	if (c>0) cellType=1;
}
if (cellType==3) //dorsal endoderm
{	radius=0.8;
    cellType=0;	
	c=sensorReaction[3]-sensorReaction[4];
	if (c<0) cellType=4;
}
if (cellType==4) //ventral endoderm
{   radius=0.7;
    cellType=7;
	c=sensorReaction[3]-sensorReaction[4];
	if (c>0) cellType=3;
}
if (cellType==3 || cellType==4)
{
  if (EnvironmentalAccess>0)
	{  cellType=0;
	}
}
