if (cellType==0)
{  if (numDivisions<10)
		{ resilience=0.5; SpawnWherever(); } // Первичная сфера
		else
		{	if (EnvironmentalAccess>0.1 && sensorReaction[2]==0)
			{	if (sensorReaction[0]==0)
				{	Spill(0); 	cellType=1; } // Спинной организатор
				else if (sensorReaction[0]<0.081 && sensorReaction[1]==0)
				{	Spill(1); cellType=1; } // Брюшной организатор	
				else if (Math.Abs(sensorReaction[0]-sensorReaction[1])<0.02) // на эквтре 
				{	if (sensorReaction[2]==0)
					{	Spill(2);	cellType=1; } // Передний организатор			
				}				
			}
			if (sensorReaction[2]>0)  // Когда три организатора уже включились
			{	if (sensorReaction[2]<0.17)
				{	if (sensorReaction[0]>sensorReaction[1])
					{	cellType=2; }// Теменная-затылочная часть головы (зеленая)	
					else
					{	cellType=3; }// Нижняя часть головы (малиновая)	
				}
				else
				{	cellType=4; } // Передняя часть головы (синяя)	
			}		
  }
}
else if (cellType==2 || cellType==3)  // спинноголовные зеленые клетки или брюшноголовные малиновые
{		if (sensorReaction[0]<sensorReaction[1])
		{	cellType=3; } // Спинноголовные клетки превращаются в брюшноголовные, если перекос	
		if (sensorReaction[0]>sensorReaction[1])
		{	cellType=2; } // Брюшноголовные превращаются в спинноголовные, если перекос	
		if (sensorReaction[2] > 0 && sensorReaction[2]<0.079 && sensorReaction[3]<0.01)
		{   // Когда три организатора включились, и если мы далеко от переднего полюса,
		    // и если слаб сигнал от "орг-ра с-та", то превратись в орг-р с-та (темно-зеленая клетка, выделяющая 3)
			cellType=5; Spill(3); numDivisions=0;
		}
}
else if (cellType==5)  // Стволовые клетки сегмента (темно-зеленые)		
{		if (numDivisions==0)
		{	SpawnGradient(2,false); // Организатор сегмента делится один раз
			holdingPosition=true;	}
		else
		{	if (!secrettingNow[3]) // Кроме организатора
			{	if (numDivisions<9) // Рост зачатка сегмента
				{	int i;
					if (sensorReaction[0]>sensorReaction[1]) // размножаются вдоль прод. оси тела
					{	i=0; }
					else
					{	i=1; }
					SpawnGradient(i,true,0); // делится к в-ву, которого больше
					MoveGradient(i,true,false,0.8); // движется от в-ва, которого больше
				}
				else  // Дифференцировка зачатка сегмента
				{   if (EnvironmentalAccess>0.1)
					{      if (sensorReaction[0]>sensorReaction[1])
						{	cellType=6; } // Спинной эпителий сегмента (бордовый)
						else
						{	cellType=7; } // Брюшной эпителий сегмента (белесый)
					}
					else
					{	cellType=8;	} // Паренхима сегмента (розовая)
				}
			}
		}
}
else if (cellType==6)  // Спинной эпителий сегмента (бордовый)
{		if (!secrettingNow[6])
		{	secretLevel[6]=0.01; // Все клетки 6 выделяют 6
			Spill(6);	}	
		if (EnvironmentalAccess>0.01)  // Стабилизирующая редифференцировка сегмента
		{	if (sensorReaction[0]<=sensorReaction[1])
			{	cellType=7;	}
		}
		else
		{	cellType=8;	}
		if (sensorReaction[3]<0.065) // Если ушли далеко от организатора сегмента
		{	if (sensorReaction[2]>0.003) // контроль длины тела
			{ cellType=5; Spill(3); numDivisions=0;} // рекурсия - запуск программы формирования сегмента
			else if (sensorReaction[9]==0)
			{	Spill(9);} // Хвост подает сигнал окончания роста тела. 
		}
}
else if (cellType==7) // Брюшной эпителий сегмента (белесый)
{		if (secrettingNow[6])
		{	DeSpill(6);	} // Стабилизирующая редифференцировка
		if (EnvironmentalAccess>0.01)
		{	if (sensorReaction[0]>sensorReaction[1])
			{	cellType=6;	}			
		}
		else
		{	cellType=8;	}
		if (sensorReaction[3]<0.065) // Повтор, как для типа 6 - рекурсия, новый сегмент
		{	if (sensorReaction[2]>0.003)
			{	cellType=5;  Spill(3);	numDivisions=0;	}
			else if (sensorReaction[9]==0)
			{	Spill(9);	}
		}		
if (sensorReaction[8]<0.02 && sensorReaction[6]>0.02 && sensorReaction[3]<0.23 && sensorReaction[9]>0 && sensorReaction[2]>0.0021)
		{	cellType=9;    Spill(8);	}// Зачатки конечностей
}
else if (cellType==8)  // Паренхима сегмента
{		if (EnvironmentalAccess>0.01) // Стабилизирующая редифференцировка
		{	if (sensorReaction[0]>sensorReaction[1])
			{	cellType=6;	}
			else
			{	cellType=7;	}
		}		
}
else if (cellType==9) // Клетки конечности концевые, которые еще сами не делились
{		if (sensorReaction[3]>0.03) // Растут от организатора сегмента
		{	SpawnGradient(3,false);	}
		cellType=10; // Клетки конечности не концевые, уже поделившиеся
}
color=cellType;
