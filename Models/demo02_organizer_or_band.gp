// Появление организатора на боку
// 

resilience = 0.4;
if (numDivisions<10)
   {SpawnWherever();}
   else
   {
	if (EnvironmentalAccess>0.2 && sensorReaction[0]==0)
	    {Spill(0);
		 cellType=1; }
	if (sensorReaction[1]==0 && EnvironmentalAccess>0 && sensorReaction[0]>0.148 && sensorReaction[0]<0.149)
//	if (EnvironmentalAccess>0 && sensorReaction[0]>0.148 && sensorReaction[0]<0.149)
	    {Spill(1);
		cellType=2;}
   
   }




