resilience=0.6;
cellType=cellType;
if (cellType==0)
{
	if (numDivisions<9)
	{
		SpawnWherever();
	}
	else
	{
		if (sensorReaction[11]==0 && EnvironmentalAccess>0.2)
		{
			Spill(11);
			cellType=5;
		} //ventral
		if (sensorReaction[12]==0 && EnvironmentalAccess>0.2 && sensorReaction[11]<0.091)
		{
			Spill(12);
			cellType=5;
		} //dorsal
		double Diff = sensorReaction[11]-sensorReaction[12];
		if (sensorReaction[11]>0 && sensorReaction[12]>0 && Diff<0.02 && Diff>-0.02 && sensorReaction[10]==0 && EnvironmentalAccess>0.2)
		{
			Spill(10);
			cellType=5;
			secretLevel[0]=0.05;
		} //anterior
		if (sensorReaction[5]==0 && EnvironmentalAccess>0.2 && sensorReaction[10]<0.085 && sensorReaction[10]>0)
		{
			Spill(5);
			cellType=5;
		} //posterior
		if (sensorReaction[5]>(sensorReaction[10]-0.06) && sensorReaction[5]!=0 && sensorReaction[10]!=0 )
		{
			radius=0.95;
		}
		if (radius==0.95 && EnvironmentalAccess>0.4)
		{
			MoveToTheCrowd(false,0.3);
		}
		if (radius==0.95 && !IsMoving && cellType==0)
		{
			if (sensorReaction[11]>sensorReaction[12])
			{
				cellType=1;
			} // posterior ventral cells
			else
			{
				cellType=2;
			} // posterior dorsal cells
		}
		if (sensorReaction[5]>0 && radius==1)
		{
			if (sensorReaction[11]>sensorReaction[12])
			{
				cellType=3;
			} // anterior ventral cells
			else
			{
				cellType=4;
			} // anterior dorsal cells
		}
	}
}
if (cellType==1 || cellType==2) // posterior
{
	if (sensorReaction[5]>0.15 && EnvironmentalAccess>0.1 )
	{
		SpawnWherever();
	}
	if (sensorReaction[5]<=0.15 && EnvironmentalAccess>0.16 && sensorReaction[4]==0)
	{
		//MoveToTheCrowd(false,0.2);
	}
	if (cellType==1 && sensorReaction[12]>sensorReaction[11])
	{
		cellType=2;
	}
	if (cellType==2 && sensorReaction[12]<=sensorReaction[11])
	{
		cellType=1;
	}
	if (cellType==1)
	{
		if (sensorReaction[0]>0.1)
		{
			if (sensorReaction[9]>0.25)
			{
				cellType=7;
			}
			else
			{
				cellType=8;
			}
		}
	}
	if (cellType==2)
	{
		if (!secrettingNow[6] && sensorReaction[6]<0.08)
		{
			secretLevel[6]=0.01;
			Spill(6);
		}
	}
}
if (cellType==5) //basic organizer
{
	if (EnvironmentalAccess<0.1)
	{
		//MoveFromTheCrowd(true,0.6);
		if (sensorReaction[11]>sensorReaction[12])
		{
			MoveGradient(11,true,false,0.6);
		}
		else
		{
			MoveGradient(12,true,false,0.6);
		}
	}
	if (secrettingNow[5] && sensorReaction[10]<0.003)
	{
		DeSpill(5);
		Spill(4);
		cellType=6;
	}
	if (secrettingNow[10])
	{
		if (sensorReaction[9]>0)
		{
			if (secrettingNow[0])
			{
				DeSpill(0);
			}
			secretLevel[0]*=1.05;
			Spill(0);
		}
		else
		{
			if (secrettingNow[0])
			{
				DeSpill(0);
			}
		}
	}
}
if (cellType==6)
{
	if (sensorReaction[0]<0.1)
	{
		if (secrettingNow[9])
		{
			DeSpill(9);
		}
		secretLevel[9]+=0.05;
		if (secretLevel[9]>0.8)
		{
			secretLevel[9]=0.1;
		}
		Spill(9);
	}
	else
	{
		DeSpill(9);
		cellType=5;
	}
}
if (cellType==7)
{
	if (!secrettingNow[17])
	{
		secretLevel[17]=0.05;
		Spill(17);
	}
}
else if (cellType==8)
{
	if (sensorReaction[0]==0 && EnvironmentalAccess>0)
	{
		if (sensorReaction[6]>0.04 && sensorReaction[16]<0.04 && sensorReaction[17]<0.1)
		{
			Spill(16);
			cellType=10;
					
		}
	}
}
else if (cellType==10)
{	
	if (sensorReaction[16]>0.005)
	{		
		polarization=FromTheCrowd;
		polarization=polarization.Normalize();	
		SpawnPolarization(true);
	}	
	cellType=11;	
}
