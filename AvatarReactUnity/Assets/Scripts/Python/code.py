import pickle
import pandas as pd
from sklearn import svm
import sys

#引数
args = sys.argv

#学習したモデルの抽出
with open('C:/Users/imd-lab/Desktop/2022TSUNDERE/VirtualCafe/AvatarReactUnity/Assets/Scripts/Python/model_selfHigh.pickle', mode='rb') as f:
    model = pickle.load(f)

#評価データ
data = pd.DataFrame([[args[1],args[2],args[3],args[4]]], columns = ["sdQ13","miQ2", "muQ22", "msQ32"])

#モデルを用いた予測
ans = model.predict(data)
if ans == 0:
    print("deredere")
if ans == 1:
    print("tsundere")